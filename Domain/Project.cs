using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Domain
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid UniqueId { get; set; }

        [Key]
        [Column(TypeName = "varchar(25)")]
        public string ProjectKey { get; set; }

        [Column(TypeName = "varchar(75)")]
        public string Name { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Desc { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Brand { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string Status { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DtCreated { get; set; }
    }
}
