using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Services
{
    public class FileStorageLocal : IFileStorage
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FileStorageLocal(IWebHostEnvironment webHostEnvironment,  IHttpContextAccessor httpContextAccessor)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> EditFileAsync(byte[] content, string extension, string container, string route, string contentType)
        {
            await RemoveFileAsync(container, route);
            return await SaveFileAsync(content, extension, container, contentType);
        }

        public Task RemoveFileAsync(string container, string route)
        {
            if (route != null)
            {
                var fileName = Path.GetFileName(route);
                string fileDirectory = Path.Combine(webHostEnvironment.WebRootPath, container, fileName);
                if (File.Exists(fileDirectory))
                {
                    File.Delete(fileDirectory);
                }
            }

            return Task.FromResult(0);
        }

        public async Task<string> SaveFileAsync(byte[] content, string extension, string container, string contentType)
        {
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(webHostEnvironment.WebRootPath, container);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string route = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(route, content);

            var currentUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var urlToDb = Path.Combine(currentUrl, container, fileName).Replace("\\", "/");
            return urlToDb;
        }
    }
}
