using System;
using System.Collections.Generic;
using System.Text;

namespace BackupUtil.Db
{
    public class SqlBackupSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string BackupPath { get; set; }
    }
}
