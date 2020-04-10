using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Options
{
    public class TransactionControlSettings
    {
        public int PermitUpdateLifetimeDays { get; set; }
        public string PermitUpdateStatus { get; set; }
        public string StorageType { get; set; }
        public string BlobContainer { get; set; }
        public string BlobContainerUrl { get; set; }
        public string BlobBaseStoragePath { get; set; }
        public string FileBaseStoragePath { get; set; }
        public string StorageConnectionString { get; set; }
        public string AcceptedFileTypes { get; set; }
    }
}
