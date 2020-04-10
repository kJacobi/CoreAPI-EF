using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Contracts.V1.Requests
{
    public class Req_CreateProject
    {
        [Required]
        public string ProjectKey { get; set; }
        [Required]
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Brand { get; set; }
        public char Status { get; set; }
    }
}
