using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BackupUtil.Db
{
    public class SqlBackup : IDbBackup
    {
        private readonly IOptions<SqlBackupSettings> _settings;
        private readonly ILogger<SqlBackup> _logger;

        private const int CommandTimeout = 1800;

        public SqlBackup(IOptions<SqlBackupSettings> settings,
            ILogger<SqlBackup> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task BackupAsync(BackupType type)
        {
            var databases = _settings.Value.GetDatabases(); 

            if (databases == null || databases.Length == 0)
            {
                _logger.LogInformation("No databases set up");
                return;
            }

            foreach (var database in databases)
            {
                var extension = "bak";
                if (type == BackupType.TransactionLog)
                {
                    extension = "trn";
                }
                var filename = $"{database}-{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}-{type}.{extension}";
                var backupPath = $"{_settings.Value.BackupPath}/{filename}";
                var backupName = $"{database}-{type}";
                var backupType = "DATABASE";
                if (type == BackupType.TransactionLog)
                {
                    backupType = "LOG";
                }
                var diff = "";
                if (type == BackupType.Differential)
                {
                    diff = "DIFFERENTIAL, ";
                }
                var sql = $"BACKUP {backupType} [{database}] TO DISK = @backupPath WITH {diff}NOFORMAT, NOINIT, NAME = @backupName, SKIP, NOREWIND, NOUNLOAD, STATS = 10";
                _logger.LogInformation("Backing up {Database} with type {Type}", database, type);
                await ExecuteAsync(sql, new SqlParameter("backupPath", backupPath), new SqlParameter("backupName", backupName));
                _logger.LogInformation("Back up completed");
            }            
        }

        private async Task<int> ExecuteAsync(string sql, params SqlParameter[] sqlParameters)
        {
            using (var connection = new SqlConnection(_settings.Value.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandTimeout = CommandTimeout;
                    command.Parameters.AddRange(sqlParameters);
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
