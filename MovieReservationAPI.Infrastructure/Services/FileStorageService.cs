using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieReservationAPI.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Infrastructure.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(
            Stream fileStream, string fileName, string contentType);
        Task<bool> DeleteFileAsync(string fileUrl);
        Task<string> GetFileUrlAsync(string fileName);
    }

    public class FileStorageService : IFileStorageService
    {
        private readonly AzureStorageSettings _settings;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(
            IOptions<AzureStorageSettings> settings,
            ILogger<FileStorageService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(
            Stream fileStream, string fileName, string contentType)
        {
            try
            {
                var containerClient = new BlobContainerClient(
                    _settings.ConnectionString, _settings.ContainerName);

                await containerClient.CreateIfNotExistsAsync(
                    PublicAccessType.Blob);

                var uniqueFileName =
                    $"{Guid.NewGuid():N}_{fileName}";
                var blobClient = containerClient.GetBlobClient(uniqueFileName);

                var uploadOptions = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                };

                await blobClient.UploadAsync(
                    fileStream, uploadOptions);

                _logger.LogInformation(
                    "File uploaded: {FileName}", uniqueFileName);

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File upload failed: {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                var uri = new Uri(fileUrl);
                var fileName = Path.GetFileName(uri.LocalPath);

                var containerClient = new BlobContainerClient(
                    _settings.ConnectionString, _settings.ContainerName);

                var blobClient = containerClient.GetBlobClient(fileName);
                var result = await blobClient.DeleteIfExistsAsync();

                return result.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File delete failed: {FileUrl}", fileUrl);
                return false;
            }
        }

        public Task<string> GetFileUrlAsync(string fileName)
        {
            var containerClient = new BlobContainerClient(
                _settings.ConnectionString, _settings.ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            return Task.FromResult(blobClient.Uri.ToString());
        }
    }
}
