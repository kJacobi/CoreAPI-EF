using CoreAPI_EF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string username, string password, string email, string role);
        Task<AuthResult> LoginAsync(string username, string password);
        Task<AuthResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
