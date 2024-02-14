using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SnatchItAPI.Models;

namespace SnatchItAPI.Data;

public class AzureBlobService
{
    private readonly string _storageAccount;
    private readonly string _accessKey;
    public readonly IConfiguration _config;
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobService(IConfiguration config)
    {
        _config = config;
        _storageAccount = _config["StorageAccount:StorageAccountName"] ?? "";
        _accessKey = _config["StorageAccount:StorageAccountKey"] ?? "";

        StorageSharedKeyCredential credential = new StorageSharedKeyCredential(_storageAccount, _accessKey);
        string blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
    }

    public async Task<IEnumerable<BlobContainerDto>> ListBlobContainersAsync()
    {
        var containerDtos = new List<BlobContainerDto>();
        var containers = _blobServiceClient.GetBlobContainersAsync();
        await foreach (var container in containers)
        {
            containerDtos.Add(new BlobContainerDto { Name = container.Name });
            // Populate other properties as needed
        }
        return containerDtos;
    }

    public async Task<IEnumerable<Uri>> ListBlobUrisAsync()
        {
            var blobUris = new List<Uri>();
            var blobContainer = _blobServiceClient.GetBlobContainerClient("democontainer");

            await foreach (BlobItem blobItem in blobContainer.GetBlobsAsync())
            {
                var blobClient = blobContainer.GetBlobClient(blobItem.Name);
                blobUris.Add(blobClient.Uri);
            }

            return blobUris;
        }


    public async Task<List<Uri>> UploadFilesAsync(Stream fileStream, string fileName)
    {
        var blobUris = new List<Uri>();
        var blobContainer = _blobServiceClient.GetBlobContainerClient("democontainer");

        var blob = blobContainer.GetBlobClient(fileName);
        await blob.UploadAsync(fileStream, overwrite: true);
        blobUris.Add(blob.Uri);

        return blobUris;
    }

    public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("democontainer");
            var blobClient = blobContainer.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadAsync();
                return downloadInfo.Value.Content;
            }
            else
            {
                throw new FileNotFoundException("Blob not found", fileName);
            }
        }

}