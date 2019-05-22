using Autofac;
using BackupUtil.Db;
using BackupUtil.Jobs;
using BackupUtil.Storage;
using Microsoft.Extensions.Hosting;

namespace BackupUtil.Infrastructure.Autofac
{
    class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MainWorker>().As<IHostedService>().SingleInstance();
            builder.RegisterType<CoreLogProvider>();

            builder.RegisterType<DbBackupJob>();
            builder.RegisterType<BackupSchedulerJob>();
            builder.RegisterType<SqlBackup>().As<IDbBackup>();
            builder.RegisterType<AzureStorage>().As<IStorage>();
        }
    }
}
