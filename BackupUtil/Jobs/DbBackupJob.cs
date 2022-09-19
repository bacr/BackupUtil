using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using BackupUtil.Db;
using BackupUtil.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace BackupUtil.Jobs
{
    public class DbBackupJob : IJob
    {
        private readonly IDbBackup _dbBackup;
        private readonly IStorage _storage;
        private readonly IOptions<DbBackupJobSettings> _settings;
        private readonly ILogger<DbBackupJob> _logger;

        private const string ArchiveExtension = "zip";

        public DbBackupJob(IDbBackup dbBackup, 
            IStorage storage, 
            IOptions<DbBackupJobSettings> settings,
            ILogger<DbBackupJob> logger)
        {
            _dbBackup = dbBackup;
            _storage = storage;
            _settings = settings;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var backupType = GetBackupType(context.MergedJobDataMap);
            await _dbBackup.BackupAsync(backupType);
            if (string.IsNullOrEmpty(_settings.Value.BackupPath))
            {
                throw new Exception("BackupPath is not specified");
            }
            if (string.IsNullOrEmpty(_settings.Value.StoragePath))
            {
                throw new Exception("StoragePath is not specified");
            }
            await BackupAsync(_settings.Value.BackupPath, _settings.Value.StoragePath);
        }

        private BackupType GetBackupType(JobDataMap jobDataMap)
        {
            var backupType = jobDataMap.GetString("backupType");
            if (backupType == null)
            {
                throw new Exception("Value backupType not found");
            }
            return Enum.Parse<BackupType>(backupType);
        }

        private async Task BackupAsync(string sourcePath, string destinationPath)
        {
            var files = Directory.GetFiles(sourcePath);
            foreach (var file in files)
            {
                var fileName = file;
                if (_settings.Value.Archive)
                {
                    fileName = Archive(fileName);
                }
                await _storage.Store(fileName, destinationPath);
                File.Delete(fileName);
            }
        }

        private string Archive(string filePath)
        {
            var zipFilePath = Path.ChangeExtension(filePath, ArchiveExtension);
            if (filePath == zipFilePath)
            {
                return filePath;
            }

            _logger.LogInformation($"Archiving {filePath}");
            
            var fileName = Path.GetFileName(filePath);

            using (var zipFile = new FileStream(zipFilePath, FileMode.Create))
            {
                using (var archive = new ZipArchive(zipFile, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(filePath, fileName);
                }
            }

            File.Delete(filePath);

            _logger.LogInformation($"Archive completed");

            return zipFilePath;
        }
    }
}
