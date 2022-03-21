using System;
using System.Collections.Generic;
using System.Text;

namespace BackupUtil.Db
{
    public class SqlBackupSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string[] Databases { get; set; }
        public string BackupPath { get; set; }

        public string[] GetDatabases()
        {
            if (Databases != null)
            {
                return Databases;
            }
            if (Database != null)
            {
                return new[] { Database };
            }
            return null;
        }
    }
}
