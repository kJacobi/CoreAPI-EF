using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Contracts.V1.Responses
{
    public class Res_Common
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
