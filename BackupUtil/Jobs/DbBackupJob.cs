using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BackupUtil.Db;
using Quartz;

namespace BackupUtil.Jobs
{
    public class DbBackupJob : IJob
    {
        private readonly IDbBackup _dbBackup;

        public DbBackupJob(IDbBackup dbBackup)
        {
            _dbBackup = dbBackup;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var backupType = Enum.Parse<BackupType>(context.JobDetail.JobDataMap.GetString("backupType"));
            await _dbBackup.BackupAsync(backupType);
        }
    }
}
