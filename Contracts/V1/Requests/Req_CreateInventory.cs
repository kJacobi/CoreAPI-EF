using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Contracts.V1.Requests
{
    public class Req_CreateInventory
    {
        [Required]
        public string ProjectKey { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]
        public string Name { get; set; }

        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Status { get; set; }
        public int Balance { get; set; }
        public int Allocation { get; set; }
        public int Usage { get; set; }
    }
}
