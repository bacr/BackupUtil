using System;
using System.Collections.Generic;
using System.Text;

namespace BackupUtil.Jobs
{
    public class DbBackupJobSettings
    {
        public string BackupPath { get; set; }
        public string StoragePath { get; set; }
        public bool Archive { get; set; }
    }
}
