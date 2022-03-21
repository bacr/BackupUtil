using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using BackupUtil.Infrastructure;

namespace BackupUtil
{
    class MainWorker : IHostedService
    {
        private readonly IScheduler _scheduler;

        public MainWorker(CoreLogProvider coreLogProvider)
        {
            LogProvider.SetCurrentLogProvider(coreLogProvider);
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().Result;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(true, cancellationToken);
        }
    }
}
