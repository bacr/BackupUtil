using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BackupUtil.Storage
{
    public class AzureStorageSettings
    {
        public string? ConnectionString { get; set; }
        public string? Container { get; set; }
    }
}
