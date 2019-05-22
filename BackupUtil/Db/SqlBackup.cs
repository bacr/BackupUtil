using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BackupUtil.Db
{
    public class SqlBackup : IDbBackup
    {
        private readonly IOptions<SqlBackupSettings> _settings;
        private readonly ILogger<SqlBackup> _logger;

        public SqlBackup(IOptions<SqlBackupSettings> settings,
            ILogger<SqlBackup> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task BackupAsync(BackupType type)
        {
            var extension = "bak";
            if (type == BackupType.TransactionLog)
            {
                extension = "trn";
            }
            var filename = $"{_settings.Value.Database}-{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}-{type}.{extension}";
            var backupPath = $"{_settings.Value.BackupPath}/{filename}";
            var backupName = $"{_settings.Value.Database}-{type}";
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
            var sql = $"BACKUP {backupType} [{_settings.Value.Database}] TO DISK = @backupPath WITH {diff}NOFORMAT, NOINIT, NAME = @backupName, SKIP, NOREWIND, NOUNLOAD, STATS = 10";
            _logger.LogInformation($"Backing up {_settings.Value.Database} with type {type}");
            await ExecuteAsync(sql, new SqlParameter("backupPath", backupPath), new SqlParameter("backupName", backupName));
            _logger.LogInformation($"Back up completed");
        }

        private async Task<int> ExecuteAsync(string sql, params SqlParameter[] sqlParameters)
        {
            using (var connection = new SqlConnection(_settings.Value.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddRange(sqlParameters);
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
