using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string bucketName, CancellationToken cancellationToken);

        Task DeleteAsync(string fileName, string bucketName, CancellationToken cancellationToken);
    }
}
