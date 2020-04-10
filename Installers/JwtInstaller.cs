using CoreAPI_EF.Interfaces;
using CoreAPI_EF.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoreAPI_EF.Installers
{
    public class JwtInstaller : IInstaller
    {
        // Below code for using Tokens and not having Refresh Tokens
        //public void InstallServices(IServiceCollection services, IConfiguration configuration)
        //{
        //    var jwtSettings = new JwtSettings();
        //    configuration.Bind(nameof(jwtSettings), jwtSettings);
        //    services.AddSingleton(jwtSettings);

        //    services.AddAuthentication(x =>
        //    {
        //        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        //    })
        //    .AddJwtBearer(x =>
        //    {
        //        x.SaveToken = true;
        //        x.RequireHttpsMetadata = false;
        //        x.TokenValidationParameters = new TokenValidationParameters()
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            RequireExpirationTime = false,
        //            ValidateLifetime = true,
        //            RoleClaimType = ClaimTypes.Role
        //        };
        //    });
        //}


        // Below code for using Tokens and additional option of having Refresh Tokens
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            var TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                RoleClaimType = ClaimTypes.Role
            };

            services.AddSingleton(TokenValidationParameters);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = TokenValidationParameters;
            });
        }
    }
}

