using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Domain
{
    public class Tbl_Reference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string TblName { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string ColName { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string Value { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Desc { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DtCreated { get; set; }
    }
}