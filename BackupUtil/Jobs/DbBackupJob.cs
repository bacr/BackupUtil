using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BackupUtil.Db;
using BackupUtil.Storage;
using Microsoft.Extensions.Options;
using Quartz;

namespace BackupUtil.Jobs
{
    public class DbBackupJob : IJob
    {
        private readonly IDbBackup _dbBackup;
        private readonly IStorage _storage;
        private readonly IOptions<DbBackupJobSettings> _settings;

        public DbBackupJob(IDbBackup dbBackup, 
            IStorage storage, 
            IOptions<DbBackupJobSettings> settings)
        {
            _dbBackup = dbBackup;
            _storage = storage;
            _settings = settings;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var backupType = Enum.Parse<BackupType>(context.MergedJobDataMap.GetString("backupType"));
            await _dbBackup.BackupAsync(backupType);
            await _storage.BackupAsync(_settings.Value.BackupPath, _settings.Value.StoragePath);
        }
    }
}
