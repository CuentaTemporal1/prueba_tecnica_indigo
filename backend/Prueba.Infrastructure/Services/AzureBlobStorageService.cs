using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Prueba.Application.Interfaces;
using System;
using System.IO;

namespace Prueba.Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string _rootFolder;
        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["BlobStorage:ConnectionString"];
            var containerName = configuration["BlobStorage:ContainerName"];

            _rootFolder = configuration["BlobStorage:RootFolder"] ?? string.Empty;

            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.None);
        }

        private string BuildBlobPath(string relativePath)
        {
            if (string.IsNullOrEmpty(_rootFolder))
            {
                return relativePath.TrimStart('/');
            }

            return $"{_rootFolder.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        }

        public async Task<string> UploadAsync(Stream stream, string path, string contentType)
        {
            string fullPath = BuildBlobPath(path);

            var blobClient = _containerClient.GetBlobClient(fullPath);

            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });

            return path;
        }

        public async Task DeleteAsync(string path)
        {
            string fullPath = BuildBlobPath(path);

            var blobClient = _containerClient.GetBlobClient(fullPath);
            await blobClient.DeleteIfExistsAsync();
        }

        public string GetFileUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            string fullPath = BuildBlobPath(path);
            var blobClient = _containerClient.GetBlobClient(fullPath);

            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    Protocol = SasProtocol.Https,
                    Version = "2023-11-03",
                    BlobContainerName = _containerClient.Name,
                    BlobName = blobClient.Name,
                    Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }

            throw new InvalidOperationException("No se pudo generar una URL SAS. Verifique la connection string de Azure Storage.");
        }
    }
}