using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Contracts.V1.Requests
{
    public class Req_UpdateInventory
    {
        public string ProjectKey { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Status { get; set; }
        public string DtStart { get; set; }
        public string DtEnd { get; set; }
        public string Balance { get; set; }
        public string Allocation { get; set; }
        public string Usage { get; set; }
    }
}
