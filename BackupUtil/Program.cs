using System;
using System.Configuration;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BackupUtil.Infrastructure;
using BackupUtil.Db;
using BackupUtil.Jobs;
using BackupUtil.Storage;

namespace BackupUtil
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    config.AddJsonFile("appsettings.local.json", optional: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<SqlBackupSettings>(hostContext.Configuration.GetSection("SqlBackup"));
                    services.Configure<AzureStorageSettings>(hostContext.Configuration.GetSection("AzureStorage"));
                    services.Configure<DbBackupJobSettings>(hostContext.Configuration.GetSection("DbBackupJob"));
                    services.Configure<BackupSchedulerJobSettings>(hostContext.Configuration.GetSection("BackupSchedulerJob"));
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((hostContext, container) =>
                {
                    AutofacConfig.ConfigureAutofac(container, hostContext.Configuration);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole(c =>
                    {
                        c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                    });
                });

            await builder.RunConsoleAsync();
        }
    }
}
