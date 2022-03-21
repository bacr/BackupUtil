using System;
using System.Collections.Generic;
using System.Text;
using BackupUtil.Db;

namespace BackupUtil.Services
{
    public class BackupSchedulerSettings
    {
        public List<DbBackupItem> Db { get; set; }
    }

    public class DbBackupItem
    {
        public BackupType Type { get; set; }
        public string Cron { get; set; }
    }
}
