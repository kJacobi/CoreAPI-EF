using AutoMapper.Configuration;
using CoreAPI_EF.Data;
using CoreAPI_EF.Interfaces;
using CoreAPI_EF.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();


            // DI Container       
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITokenHelperService, TokenHelperService>();
            services.AddScoped<ITransactionService, TransactionService>();
        }
    }
}