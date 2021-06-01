using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Services
{
    public interface IFileStorage
    {
        Task<string> EditFileAsync(byte[] content, string extension, string container, string route, string contentType);
        Task RemoveFileAsync(string container, string route );
        Task<string> SaveFileAsync(byte[] content, string extension, string container, string contentType);
    }
}
