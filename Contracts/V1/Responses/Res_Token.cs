using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Contracts.V1.Responses
{
    public class Res_Token
    {
        public bool Success
        {
            get { return true; }
        }
        public string Token { get; set; }

        public string RefreshToken { get; set; }
        public DateTime Expires { get; set; }
    }
}
