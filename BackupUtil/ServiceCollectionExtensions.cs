using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using BackupUtil.Db;
using BackupUtil.Infrastructure;
using BackupUtil.Jobs;
using BackupUtil.Services;
using BackupUtil.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace BackupUtil
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBackupUtil(this IServiceCollection services, IConfiguration configuration)
        {
            var assemblies = GetAppAssemblies();
            services.AddSingleton<IMapper>(sp => new MapperConfiguration(cfg => { cfg.AddMaps(assemblies); }).CreateMapper());
            services.AddFromConfiguration(configuration, assemblies);

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
            });
            services.AddQuartzHostedService(
                q => q.WaitForJobsToComplete = true);

            services.AddHostedService<BackupSchedulerService>();

            services.AddTransient<DbBackupJob>();

            services.Configure<SqlBackupSettings>(configuration.GetSection("SqlBackup"));
            services.Configure<AzureStorageSettings>(configuration.GetSection("AzureStorage"));
            services.Configure<AWSS3StorageSettings>(configuration.GetSection("AWSS3Storage"));
            services.Configure<DbBackupJobSettings>(configuration.GetSection("DbBackupJob"));
            services.Configure<BackupSchedulerSettings>(configuration.GetSection("BackupSchedulerJob"));

            return services;
        }

        private static Assembly[] GetAppAssemblies()
        {
            var appNames = new[] { "BackupUtil" };
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => appNames.Any(n => a.FullName.Contains(n)) ||
                            a.GetReferencedAssemblies()
                                .Any(ra => appNames.Any(n => ra.FullName.Contains(n))))
                .ToArray();
        }
    }
}
