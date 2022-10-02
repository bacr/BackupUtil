using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackupUtil.Storage.FTP
{
    public class FTPStorage : IStorage
    {
        private readonly FTPStorageSettings _settings;
        private readonly ILogger<FTPStorage> _logger;

        public FTPStorage(IOptions<FTPStorageSettings> settings,
            ILogger<FTPStorage> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task Store(string filePath, string destinationPath)
        {
            var filename = Path.GetFileName(filePath);
            var destinationFile = $"{destinationPath}/{filename}";

            var client = new AsyncFtpClient(_settings.Host, _settings.User, _settings.Password, _settings.Port);

            await client.AutoConnect();

            _logger.LogInformation("Uploading {FilePath} to FTP", filePath);

            await client.UploadFile(filePath, destinationFile);

            _logger.LogInformation("Upload complete");
        }
    }
}
