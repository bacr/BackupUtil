using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupUtil.Storage.FTP
{
    public class FTPStorageSettings
    {
        public string? Host { get; set; }

        public int Port { get; set; } = 21;

        public string? User { get; set; }

        public string? Password { get; set; }
    }
}
