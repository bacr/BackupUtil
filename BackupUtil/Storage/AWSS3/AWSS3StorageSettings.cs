using System;
using System.Collections.Generic;
using System.Text;

namespace BackupUtil.Storage
{
    public class AWSS3StorageSettings
    {
        public string? AccessKey { get; set; }
        public string? SecretKey { get; set; }
        public string? BucketName { get; set; }
        public string? ServiceUrl { get; set; }
    }
}
