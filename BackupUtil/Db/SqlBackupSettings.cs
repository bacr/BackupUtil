using System;
using System.Collections.Generic;
using System.Text;

namespace BackupUtil.Db
{
    public class SqlBackupSettings
    {
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string BackupPath { get; set; }
    }
}
