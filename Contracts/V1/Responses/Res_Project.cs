using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Contracts.V1.Responses
{
    public class Res_Project
    {
        public Guid UniqueId { get; set; }
        public string ProjectKey { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Brand { get; set; }
        public char Status { get; set; }
        public DateTime DtCreated { get; set; }
    }
}
