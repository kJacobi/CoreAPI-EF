using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Data
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsEnabled { get; set; }
    }
}
