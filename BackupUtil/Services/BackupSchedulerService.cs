using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackupUtil.Jobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace BackupUtil.Services
{
    public class BackupSchedulerService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IOptions<BackupSchedulerSettings> _backupSchedulerSettings;
        private readonly ILogger<BackupSchedulerService> _logger;

        public BackupSchedulerService(ISchedulerFactory schedulerFactory,
            IOptions<BackupSchedulerSettings> backupSchedulerSettings,
            ILogger<BackupSchedulerService> logger)
        {
            _schedulerFactory = schedulerFactory;
            _backupSchedulerSettings = backupSchedulerSettings;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            foreach (var db in _backupSchedulerSettings.Value.Db)
            {
                _logger.LogInformation("Schedule {Type} backup at {Cron}", db.Type, db.Cron);
                var job = JobBuilder.Create<DbBackupJob>()
                    .Build();
                var trigger = TriggerBuilder.Create()
                    .UsingJobData("backupType", db.Type.ToString())
                    .WithCronSchedule(db.Cron)
                    .Build();
                await scheduler.ScheduleJob(job, trigger, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
