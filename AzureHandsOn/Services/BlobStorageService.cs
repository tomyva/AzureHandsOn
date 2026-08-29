using Azure.Storage.Blobs;

namespace AzureHandsOn.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("AzureStorage")
            ?? throw new InvalidOperationException(
                "AzureStorage connection string not found.");

        _containerClient = new BlobContainerClient(
            connectionString,
            "ticket-attachments");
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName)
    {
        var blobName =
            $"{Guid.NewGuid()}-{fileName}";

        var blobClient =
            _containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(
            stream,
            overwrite: false);

        return blobName;
    }
}