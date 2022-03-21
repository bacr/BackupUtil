using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackupUtil.Storage
{
    public class AWSS3Storage : IStorage
    {
        private readonly AWSS3StorageSettings _settings;
        private readonly ILogger<AWSS3Storage> _logger;

        public AWSS3Storage(IOptions<AWSS3StorageSettings> settings,
            ILogger<AWSS3Storage> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        private AmazonS3Client GetClient()
        {
            return new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, new AmazonS3Config
            {
                ServiceURL = _settings.ServiceUrl
            });
        }

        public async Task BackupAsync(string sourcePath, string destinationPath)
        {
            var files = Directory.GetFiles(sourcePath);
            foreach (var file in files)
            {
                await Store(file, destinationPath);
                File.Delete(file);
            }
        }

        private async Task Store(string filePath, string destinationPath)
        {
            var filename = Path.GetFileName(filePath);
            var destinationFile = $"{destinationPath}/{filename}";
            var client = GetClient();
            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = destinationFile,
                FilePath = filePath,
            };
            _logger.LogInformation($"Uploading {filePath} to AWS S3");
            await client.PutObjectAsync(request);
            _logger.LogInformation("Upload complete");
        }
    }
}
