using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;

namespace Net8.CoreService.Storage
{
    public interface IBlobStorageAppService
    {
        //string GetBlobStorageProvider();
        //object? GetBlobStorageConfig();
        //Task<string?> UploadFileAsync(IFormFile files);
        //Task<bool> DeleteFileAsync(string fileName);
        //Task<DtoFileResponse> DownloadFileAsync(string fileName);
    }
}
