using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Domain
{
    public class TransactionLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string LoggedByUserName { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DtLogged { get; set; }
        public int TransactionId { get; set; }
        public Guid TransactionUniqueId { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string ProjectKey { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Firstname { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Lastname { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        public DateTime? DtBirth { get; set; }

        [Column(TypeName = "varchar(75)")]
        public string Company { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Address1 { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string Address2 { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string City { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string State { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Postal { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string Country { get; set; }

        public int InventoryId { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string SKU { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Text1 { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Text2 { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Text3 { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Text4 { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Text5 { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string Text6 { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string Text7 { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string Text8 { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string Text9 { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string Text10 { get; set; }

        public DateTime? DtCreated { get; set; }
        public DateTime? DtTransaction { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string AssignedTo { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string Status { get; set; }
        public DateTime? DtPulled { get; set; }
    }
}
