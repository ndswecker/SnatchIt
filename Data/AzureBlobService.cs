using Azure.Storage;
using Azure.Storage.Blobs;

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

    public async Task ListBlobContainersAsync()
    {
        Console.WriteLine("ListBlobContainers called");
        var containers = _blobServiceClient.GetBlobContainersAsync();
        await foreach (var container in containers)
        {
            Console.WriteLine(container.Name);
        }
    }
}