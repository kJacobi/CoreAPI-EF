using CoreAPI_EF.Data;
using CoreAPI_EF.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreAPI_EF.Services
{
    public class TokenHelperService : ITokenHelperService
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenHelperService(TokenValidationParameters tokenValidationParameters, UserManager<ApplicationUser> userManager)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _userManager = userManager;
        }


        /*******************************************************
        * GetUserFromToken
        * ****************************************************/
        public async Task<ApplicationUser> GetUserFromToken(string token)
        {
            var validatedToken = GetPrincipalFromToken(token);
            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);

            return user;
        }


        /*******************************************************
        * GetPrincipalFromToken
        * ****************************************************/
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                _tokenValidationParameters.ValidateLifetime = true;

                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }


        /*******************************************************
        * IsJwtWithValidSecurityAlgorithm
        * ****************************************************/
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                     StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
