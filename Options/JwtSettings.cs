using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Options
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int TokenLifetime { get; set; }
        public int RefreshTokenLifetime { get; set; }
        public string SuperTokenExpDate { get; set; }
    }
}
