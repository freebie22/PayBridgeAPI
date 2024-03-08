using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace PayBridgeAPI.Services.AzureBlobs
{
        public class BlobService : IBlobService
        {
            private readonly BlobServiceClient _blobServiceClient;
            public BlobService(BlobServiceClient blobServiceClient)
            {
                _blobServiceClient = blobServiceClient;
            }
            public async Task<bool> DeleteBlob(string blobName)
            {
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient("paybridgecontainer");
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                return await blobClient.DeleteIfExistsAsync();
            }

            public async Task<string> GetBlob(string blobName)
            {
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient("paybridgecontainer");
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                return blobClient.Uri.AbsoluteUri;
            }

            public async Task<string> UpdateBlob(string blobName, IFormFile file)
            {
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient("paybridgecontainer");
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                var httpHeaders = new BlobHttpHeaders()
                {
                    ContentType = file.ContentType,
                };

                var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);
                if (result != null)
                {
                    return await GetBlob(blobName);
                }
                return "";
            }
        }
    }

