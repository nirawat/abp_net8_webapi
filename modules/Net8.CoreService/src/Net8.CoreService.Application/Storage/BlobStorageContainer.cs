using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;

namespace Net8.CoreService.Storage
{
    public class BlobStorageContainer : IBlobStorageAppService, ITransientDependency
    {
        //    private readonly IConfiguration _iConfiguration;
        //    private readonly IBlobContainer<BlobStorageContainer> _iBlobContainer;
        //    //private readonly ICoreRepository _iCoreRepository;
        //    private readonly string provider = string.Empty;
        //    private string containerName = string.Empty;
        //    //private readonly DtoPhysicalSetting _physicalSetting = new DtoPhysicalSetting();
        //    //private readonly DtoAzureSetting _azureSetting = new DtoAzureSetting();
        //    //private readonly DtoAwsSetting _awsSetting = new DtoAwsSetting();
        //    //private readonly DtoMinioSetting _minioSetting = new DtoMinioSetting();

        public BlobStorageContainer(IConfiguration configuration) { }

        //    public BlobStorageContainer(
        //     IConfiguration iConfiguration,
        //     IBlobContainer<BlobStorageContainer> iBlobContainer,
        //     ICoreRepository iCoreRepository
        //     )
        //    {
        //        _iConfiguration = iConfiguration;
        //        _iBlobContainer = iBlobContainer;
        //        _iCoreRepository = iCoreRepository;

        //        provider = _iConfiguration["BlobStore:Provider"];

        //        #region "Physical"
        //        _physicalSetting.BasicPath = _iConfiguration["BlobStore:Physical:BasicPath"];
        //        #endregion

        //        #region "Azure"
        //        _azureSetting.BlobEndpoint = _iConfiguration["BlobStore:Azure:BlobEndpoint"];
        //        _azureSetting.QueueEndpoint = _iConfiguration["BlobStore:Azure:QueueEndpoint"];
        //        _azureSetting.FileEndpoint = _iConfiguration["BlobStore:Azure:FileEndpoint"];
        //        _azureSetting.TableEndPoint = _iConfiguration["BlobStore:Azure:TableEndPoint"];
        //        _azureSetting.SharedEndPoint = _iConfiguration["BlobStore:Azure:SharedEndPoint"];
        //        _azureSetting.ContainerName = _iConfiguration["BlobStore:Azure:ContainerName"];
        //        _azureSetting.CreateContainerIfNotExits = Convert.ToBoolean(_iConfiguration["BlobStore:Azure:CreateContainerIfNotExits"]);
        //        #endregion

        //        #region "AWS"
        //        _awsSetting.AccessKeyId = _iConfiguration["BlobStore:Aws:AccessKeyId"];
        //        _awsSetting.SecretAccessKey = _iConfiguration["BlobStore:Aws:SecretAccessKey"];
        //        _awsSetting.UseCredentials = Convert.ToBoolean(_iConfiguration["BlobStore:Aws:UseCredentials"]);
        //        _awsSetting.UseTemporaryCredentials = Convert.ToBoolean(_iConfiguration["BlobStore:Aws:UseTemporaryCredentials"]);
        //        _awsSetting.UseTemporaryFederatedCredentials = Convert.ToBoolean(_iConfiguration["BlobStore:Aws:UseTemporaryFederatedCredentials"]);
        //        _awsSetting.ProfileName = _iConfiguration["BlobStore:Aws:ProfileName"];
        //        _awsSetting.ProfilesLocation = _iConfiguration["BlobStore:Aws:ProfilesLocation"];
        //        _awsSetting.Region = _iConfiguration["BlobStore:Aws:Region"];
        //        _awsSetting.Policy = _iConfiguration["BlobStore:Aws:Policy"];
        //        _awsSetting.DurationSeconds = Convert.ToInt32(_iConfiguration["BlobStore:Aws:DurationSeconds"]);
        //        _awsSetting.ContainerName = _iConfiguration["BlobStore:Aws:ContainerName"];
        //        _awsSetting.CreateContainerIfNotExists = Convert.ToBoolean(_iConfiguration["BlobStore:Aws:CreateContainerIfNotExists"]);
        //        #endregion

        //        #region "Minio"
        //        _minioSetting.EndPoint = _iConfiguration["BlobStore:Minio:EndPoint"];
        //        _minioSetting.AccessKey = _iConfiguration["BlobStore:Minio:AccessKey"];
        //        _minioSetting.SecretKey = _iConfiguration["BlobStore:Minio:SecretKey"];
        //        _minioSetting.ContainerName = _iConfiguration["BlobStore:Minio:BucketName"];
        //        #endregion

        //    }

        //    public string GetBlobStorageProvider()
        //    {
        //        return provider;
        //    }

        //    public object? GetBlobStorageConfig()
        //    {
        //        object? result = null;
        //        switch (provider.ToUpper())
        //        {
        //            case "PHYSICAL":
        //                result = _physicalSetting;
        //                break;
        //            case "AZURE":
        //                result = _azureSetting;
        //                break;
        //            case "AWS":
        //                result = _awsSetting;
        //                break;
        //            case "MINIO":
        //                result = _minioSetting;
        //                break;
        //            default:
        //                break;
        //        }
        //        return result;
        //    }

        //    public async Task<string?> UploadFileAsync(IFormFile files)
        //    {
        //        try
        //        {
        //            var fileId = Guid.NewGuid();

        //            string fileExtention = Path.GetExtension(files.FileName);

        //            string fileNameOfSystem = string.Format("{0}{1}", fileId, fileExtention);

        //            #region "Upload File"

        //            switch (provider.ToUpper())
        //            {
        //                case "AWS":
        //                    using (var s3Client = new AmazonS3Client(
        //                         _awsSetting.AccessKeyId,
        //                         _awsSetting.SecretAccessKey,
        //                         RegionEndpoint.GetBySystemName(_awsSetting.Region)))
        //                    {
        //                        var transferUtility = new TransferUtility(s3Client);

        //                        using (var memoryStream = new MemoryStream())
        //                        {
        //                            await files.CopyToAsync(memoryStream);

        //                            var uploadOptions = new TransferUtilityUploadRequest
        //                            {
        //                                InputStream = memoryStream,
        //                                BucketName = _awsSetting.ContainerName,
        //                                Key = fileNameOfSystem
        //                            };

        //                            await transferUtility.UploadAsync(uploadOptions);
        //                        }
        //                    };
        //                    containerName = _awsSetting.ContainerName;
        //                    break;
        //                case "MINIO":
        //                    using (var memoryStream = new MemoryStream())
        //                    {
        //                        await files.CopyToAsync(memoryStream).ConfigureAwait(false);
        //                        await _iBlobContainer.SaveAsync(fileNameOfSystem, memoryStream.ToArray(), true).ConfigureAwait(false);
        //                    }
        //                    containerName = _minioSetting.ContainerName;
        //                    break;
        //                default:
        //                    using (var memoryStream = new MemoryStream())
        //                    {
        //                        await files.CopyToAsync(memoryStream);
        //                        await _iBlobContainer.SaveAsync(fileNameOfSystem, memoryStream.ToArray(), true);
        //                    }
        //                    containerName = provider.ToUpper() == "AZURE" ? _azureSetting.ContainerName : _physicalSetting.BasicPath;
        //                    break;
        //            }

        //            #endregion

        //            #region "Save History Database"

        //            var newFileUpload = new TrnBlobStorageHistory()
        //            {
        //                FileId = fileId,
        //                FileName = files.FileName,
        //                BlobFileName = fileNameOfSystem,
        //                FileContentType = files.ContentType,
        //                FileSize = (files.Length / 1024),
        //                Provider = provider.ToUpper(),
        //                ContainerName = containerName,
        //                CreatedBy = "sys",
        //                CreatedDate = DateTime.Now
        //            };

        //            await _iCoreRepository.AddTrnBlobStorageHistoryAsync(newFileUpload);

        //            return fileNameOfSystem;

        //            #endregion

        //        }
        //        catch (AmazonS3Exception ex)
        //        {
        //            ex.Message.ToString();
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.Message.ToString();
        //        }
        //        return null;
        //    }

        //    public async Task<DtoFileResponse> DownloadFileAsync(string fileName)
        //    {
        //        try
        //        {
        //            string contentType = "";
        //            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);

        //            if (provider.ToUpper() == "AWS")
        //            {
        //                using (var s3Client = new AmazonS3Client(_awsSetting.AccessKeyId, _awsSetting.SecretAccessKey, RegionEndpoint.GetBySystemName(_awsSetting.Region)))
        //                {
        //                    var request = new GetObjectRequest
        //                    {
        //                        BucketName = _awsSetting.ContainerName,
        //                        Key = fileName
        //                    };

        //                    using (var response = await s3Client.GetObjectAsync(request))
        //                    using (var responseStream = response.ResponseStream)
        //                    using (var memoryStream = new MemoryStream())
        //                    {
        //                        await responseStream.CopyToAsync(memoryStream);
        //                        return new DtoFileResponse()
        //                        {
        //                            FileName = fileName,
        //                            ContentType = contentType,
        //                            FileByte = memoryStream.ToArray()
        //                        };
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var fileByte = await _iBlobContainer.GetAllBytesOrNullAsync(fileName);
        //                return new DtoFileResponse()
        //                {
        //                    FileName = fileName,
        //                    ContentType = contentType,
        //                    FileByte = fileByte.ToArray()
        //                };
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            return new DtoFileResponse()
        //            {
        //                ErrorMessage = ex.Message.ToString()
        //            };
        //        }
        //    }

        //    public async Task<bool> DeleteFileAsync(string BlobfileName)
        //    {
        //        try
        //        {

        //            if (provider.ToUpper() == "AWS")
        //            {
        //                using (var s3Client = new AmazonS3Client(_awsSetting.AccessKeyId, _awsSetting.SecretAccessKey, RegionEndpoint.GetBySystemName(_awsSetting.Region)))
        //                {
        //                    var request = new DeleteObjectRequest
        //                    {
        //                        BucketName = _awsSetting.ContainerName,
        //                        Key = BlobfileName
        //                    };
        //                    await s3Client.DeleteObjectAsync(request);
        //                }
        //            }
        //            else
        //            {
        //                await _iBlobContainer.DeleteAsync(BlobfileName);
        //            }

        //            await _iCoreRepository.DeleteTrnBlobStoragesAsync(BlobfileName);

        //            return true;
        //        }
        //        catch (AmazonS3Exception ex)
        //        {
        //            ex.Message.ToString();
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.Message.ToString();
        //        }
        //        return false;
        //    }
        //}


    }
}
