using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Contracts.V1.Responses
{
    public class Res_GetTransaction
    {
        public Guid UniqueId { get; set; }
        public string ProjectKey { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }

        [DefaultValue("1900-01-01")]
        public DateTime? DtBirth { get; set; } = Convert.ToDateTime("1900-01-01");
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
        public string Country { get; set; }
        public string SKU { get; set; }
        public decimal Amount { get; set; } = 0;
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }
        public string Text5 { get; set; }
        public string Text6 { get; set; }
        public string Text7 { get; set; }
        public string Text8 { get; set; }
        public string Text9 { get; set; }
        public string Text10 { get; set; }
        public DateTime DtCreated { get; set; }
        public object ImageData { get; set; }
    }
}
