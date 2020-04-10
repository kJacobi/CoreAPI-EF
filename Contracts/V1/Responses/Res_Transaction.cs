using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Contracts.V1.Responses
{
    public class Res_Transaction
    {
        public Guid UniqueId { get; set; }
        public bool Success { get; set; }
        public IList<string> Errors { get; set; }
    }
}
