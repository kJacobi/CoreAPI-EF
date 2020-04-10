using CoreAPI_EF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreAPI_EF.Interfaces
{
    public interface ITokenHelperService
    {
        ClaimsPrincipal GetPrincipalFromToken(string token);
        Task<ApplicationUser> GetUserFromToken(string token);
    }
}
