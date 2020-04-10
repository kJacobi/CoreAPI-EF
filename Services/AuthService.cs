using CoreAPI_EF.Data;
using CoreAPI_EF.Domain;
using CoreAPI_EF.Interfaces;
using CoreAPI_EF.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoreAPI_EF.Services
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly DataContext _dataContext;
        private readonly ITokenHelperService _tokenHelperService;

        public AuthService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings, RoleManager<IdentityRole> roleManager, DataContext dataContext, ITokenHelperService tokenHelperService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _roleManager = roleManager;
            _dataContext = dataContext;
            _tokenHelperService = tokenHelperService;
        }

        public async Task<AuthResult> RegisterAsync(string username, string password, string email, string role)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "Role is invalid." }
                };

            }

            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "Username already exists." }
                };
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = username,
                IsEnabled = false
            };
            await _userManager.CreateAsync(newUser, password);
            await _userManager.AddToRoleAsync(newUser, role);

            return new AuthResult
            {
                Success = true
            };
        }

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "User does not exist." }
                };
            }
            else if (!user.IsEnabled)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "User is not enabled." }
                };
            }

            var hasValidCredentials = await _userManager.CheckPasswordAsync(user, password);
            if (!hasValidCredentials)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "User/password combination is wrong." }
                };
            }

            return await GeneratTokenResultForUserAsync(user);

        }


        private async Task<AuthResult> GeneratTokenResultForUserAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();

            var authClaims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("role", role),
                    new Claim("id", user.Id)
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret));

            DateTime tokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifetime);
            if (role == "Super")
            {
                tokenExpiration = Convert.ToDateTime(_jwtSettings.SuperTokenExpDate);
            }

            var token = new JwtSecurityToken(
                issuer: "CCG Marketing Services",
                audience: "",
                expires: tokenExpiration,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                DtCreated = DateTime.UtcNow,
                DtExpires = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenLifetime)
            };

            await _dataContext.RefreshTokens.AddAsync(refreshToken);
            await _dataContext.SaveChangesAsync();

            return new AuthResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token.ToString(),
                Expires = token.ValidTo
            };

        }

        public async Task<AuthResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = _tokenHelperService.GetPrincipalFromToken(token);
            if (validatedToken is null)
            {
                return new AuthResult { Success = false, Errors = new[] { "Invalid Token" } };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthResult { Success = false, Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = await _dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token.ToString() == refreshToken);

            if (storedRefreshToken is null)
            {
                return new AuthResult { Success = false, Errors = new[] { "This refresh token does not exist" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.DtExpires)
            {
                return new AuthResult { Success = false, Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthResult { Success = false, Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthResult { Success = false, Errors = new[] { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.Used = true;
            _dataContext.RefreshTokens.Update(storedRefreshToken);
            await _dataContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);

            return await GeneratTokenResultForUserAsync(user);
        }

    }
}
