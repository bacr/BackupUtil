using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackupUtil.Storage
{
    public class AzureStorage : IStorage
    {
        private readonly IOptions<AzureStorageSettings> _settings;
        private readonly ILogger<AzureStorage> _logger;

        public AzureStorage(IOptions<AzureStorageSettings> settings,
            ILogger<AzureStorage> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task BackupAsync(string filePath, string destinationPath)
        {
            await Store(filePath, destinationPath);
            File.Delete(filePath);
        }

        private async Task Store(string filePath, string destinationPath)
        {
            var storageAccount = CloudStorageAccount.Parse(_settings.Value.ConnectionString);
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference(_settings.Value.Container);
            var filename = Path.GetFileName(filePath);
            var destinationFile = $"{destinationPath}/{filename}";
            _logger.LogInformation($"Uploading {filePath} to Azure Storage");
            var blob = container.GetBlockBlobReference(destinationFile);
            await blob.UploadFromFileAsync(filePath);
            _logger.LogInformation("Upload complete");
        }
    }
}
