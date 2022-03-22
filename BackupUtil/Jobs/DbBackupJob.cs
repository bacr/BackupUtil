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
            var backupType = Enum.Parse<BackupType>(context.MergedJobDataMap.GetString("backupType"));
            await _dbBackup.BackupAsync(backupType);
            await BackupAsync(_settings.Value.BackupPath, _settings.Value.StoragePath);
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
