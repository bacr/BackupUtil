using System;
using System.Linq;
using System.Reflection;
using Autofac;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace BackupUtil.Infrastructure
{
    public class AutofacConfig
    {
        public static void ConfigureAutofac(ContainerBuilder builder, IConfiguration configuration)
        {
            var assemblies = GetAppAssemblies();
            builder.RegisterAssemblyModules(assemblies);
            builder.Register(c => new MapperConfiguration(cfg => { cfg.AddMaps(assemblies); }).CreateMapper())
                .As<IMapper>().SingleInstance();
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
