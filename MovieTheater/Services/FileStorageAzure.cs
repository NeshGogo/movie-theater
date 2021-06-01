using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;

namespace MovieTheater.Services
{
    public class FileStorageAzure : IFileStorage
    {
        private readonly string connectionString;

        public FileStorageAzure(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task<string> EditFileAsync(byte[] content, string extension, string container, string route, string contentType)
        {
            await RemoveFileAsync(route, container);
            return await SaveFileAsync(content, extension, container, contentType);
        }

        public async Task RemoveFileAsync(string container, string route)
        {
            if(route != null)
            {
                var account = CloudStorageAccount.Parse(connectionString);
                var client = account.CreateCloudBlobClient();
                var containerRef = client.GetContainerReference(container);

                var blobName = Path.GetFileName(route);
                var blob = containerRef.GetBlobReference(blobName);
                await blob.DeleteIfExistsAsync();
            }
        }

        public async Task<string> SaveFileAsync(byte[] content, string extension, string container, string contentType)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            var containerRef = client.GetContainerReference(container);

            await containerRef.CreateIfNotExistsAsync();
            await containerRef.SetPermissionsAsync(new BlobContainerPermissions 
            { 
                PublicAccess = BlobContainerPublicAccessType.Blob 
            });

            var fileName = $"{Guid.NewGuid()}{extension}";
            var blob = containerRef.GetBlockBlobReference(fileName);
            await blob.UploadFromByteArrayAsync(content, 0, content.Length);
            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();
            return blob.Uri.ToString();
            
        }
    }
}
