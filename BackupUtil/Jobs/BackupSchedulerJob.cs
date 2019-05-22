using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Quartz;

namespace BackupUtil.Jobs
{
    public class BackupSchedulerJob : IJob
    {
        private readonly IOptions<BackupSchedulerJobSettings> _backupSchedulerJobSettings;

        public BackupSchedulerJob(IOptions<BackupSchedulerJobSettings> backupSchedulerJobSettings)
        {
            _backupSchedulerJobSettings = backupSchedulerJobSettings;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            foreach (var db in _backupSchedulerJobSettings.Value.Db)
            {
                var job = JobBuilder.Create<DbBackupJob>()
                    .Build();
                var trigger = TriggerBuilder.Create()
                    .UsingJobData("backupType", db.Type.ToString())
                    .WithCronSchedule(db.Cron)
                    .Build();                
                await context.Scheduler.ScheduleJob(job, trigger, context.CancellationToken);
            }
        }
    }
}
