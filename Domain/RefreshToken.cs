using CoreAPI_EF.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Domain
{
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Token { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string JwtId { get; set; }

        public DateTime DtCreated { get; set; }
        public DateTime DtExpires { get; set; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [Column(TypeName = "varchar(100)")]
        public ApplicationUser User { get; set; }
    }
}
