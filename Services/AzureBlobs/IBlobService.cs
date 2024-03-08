namespace PayBridgeAPI.Services.AzureBlobs
{
    public interface IBlobService
    {
        Task<string> GetBlob(string blobName);
        Task<bool> DeleteBlob(string blobName);
        Task<string> UpdateBlob(string blobName, IFormFile file);
    }
}
