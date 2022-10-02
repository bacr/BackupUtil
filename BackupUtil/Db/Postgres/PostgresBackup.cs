using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackupUtil.Db.Postgres
{
    public class PostgresBackup : IDbBackup
    {
        private readonly IOptions<PostgresBackupSettings> _settings;
        private readonly ILogger<PostgresBackup> _logger;

        public PostgresBackup(IOptions<PostgresBackupSettings> settings,
            ILogger<PostgresBackup> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task BackupAsync(BackupType type)
        {
            if (type != BackupType.Full)
            {
                throw new ArgumentException("Only Full backup is supported", nameof(type));
            }

            // TODO: https://stackoverflow.com/questions/23026949/how-to-backup-restore-postgresql-using-code

            var databases = _settings.Value.GetDatabases();

            if (databases == null)
            {
                _logger.LogInformation($"No databases set up");
                return;
            }

            foreach (var database in databases)
            {
                var filename = $"{database}-{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}-{type}.sql";
                var backupPath = $"{_settings.Value.BackupPath}/{filename}";

                var dumpCommand =
                    $"export PGPASSWORD={_settings.Value.Password}\n" +
                    $"pg_dump -Fc -h {_settings.Value.Host} -p {_settings.Value.Port} -d {database} -U {_settings.Value.User}";

                var batchContent = $"{dumpCommand} > \"{backupPath}\"\n";

                _logger.LogInformation("Backing up {Database} with type {Type}", database, type);

                var result = await ExecuteBatch(batchContent);
                if (result != 0)
                {
                    if (File.Exists(backupPath))
                    {
                        File.Delete(backupPath);
                    }
                    throw new Exception($"Execute returned code {result}");
                }

                _logger.LogInformation("Back up completed");
            }
        }

        private async Task<int> ExecuteBatch(string batchContent)
        {
            var batFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.sh");
            try
            {
                File.WriteAllText(batFilePath, batchContent);

                var psi = new ProcessStartInfo("sh")
                {
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Arguments = batFilePath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                using var process = new Process
                {
                    StartInfo = psi,
                    EnableRaisingEvents = true
                };

                return await RunCommandAsync(process).ConfigureAwait(false);
            }
            finally
            {
                if (File.Exists(batFilePath))
                {
                    File.Delete(batFilePath);
                }
            }
        }

        private Task<int> RunCommandAsync(Process process)
        {
            var tcs = new TaskCompletionSource<int>();

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
            };
            process.OutputDataReceived += (s, ea) =>
            {
                if (!string.IsNullOrEmpty(ea.Data))
                {
                    _logger.LogInformation(ea.Data);
                }
            };
            process.ErrorDataReceived += (s, ea) =>
            {
                if (!string.IsNullOrEmpty(ea.Data))
                {
                    _logger.LogError(ea.Data);
                }
            };

            var started = process.Start();

            if (!started)
            {
                throw new InvalidOperationException("Could not run process: " + process);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }
    }
}
