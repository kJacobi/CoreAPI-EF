using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Domain
{
    public class TransactionImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid UniqueId { get; set; }

        public int TransactionId { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string StorageType { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string BlobContainer { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string BlobContainerUrl { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string Path { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string ImageName { get; set; }

        public bool Active { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DtCreated { get; set; }
    }
}
