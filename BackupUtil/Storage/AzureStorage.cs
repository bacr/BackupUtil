using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
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

        public async Task Store(string filePath, string destinationPath)
        {
            var blobServiceClient = new BlobServiceClient(_settings.Value.ConnectionString);
            var container = blobServiceClient.GetBlobContainerClient(_settings.Value.Container);
            var filename = Path.GetFileName(filePath);
            var destinationFile = $"{destinationPath}/{filename}";
            _logger.LogInformation("Uploading {FilePath} to Azure Storage", filePath);
            var blob = container.GetBlobClient(destinationFile);
            await blob.UploadAsync(filePath);
            _logger.LogInformation("Upload complete");
        }
    }
}
