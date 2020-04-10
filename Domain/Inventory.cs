using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Domain
{
    public class Inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid UniqueId { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string ProjectKey { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string SKU { get; set; }

        [Column(TypeName = "varchar(75)")]
        public string Name { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Desc1 { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Desc2 { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string Status { get; set; }

        public DateTime? DtStart { get; set; }
        public DateTime? DtEnd { get; set; }

        public int Balance { get; set; }
        public int Allocation { get; set; }
        public int Usage { get; set; }

        public int ControlType { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DtCreated { get; set; }

    }
}