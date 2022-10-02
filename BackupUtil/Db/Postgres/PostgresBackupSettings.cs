using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupUtil.Db.Postgres
{
    public class PostgresBackupSettings
    {
        public string? Database { get; set; }

        public string[]? Databases { get; set; }

        public string? Host { get; set; }

        public int Port { get; set; } = 5432;

        public string? User { get; set; }

        public string? Password { get; set; }

        public string? BackupPath { get; set; }

        public string[]? GetDatabases()
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
