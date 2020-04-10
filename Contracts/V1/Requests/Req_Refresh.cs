using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Contracts.V1.Requests
{
    public class Req_Refresh
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
