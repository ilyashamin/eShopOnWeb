using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace FunctionApp;

public class OrderItemsReserver
{
    private readonly ILogger<OrderItemsReserver> _logger;

    private readonly string _storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ishwebappstorageacc;AccountKey=CPUIQicNfzs3zP0XThdcUN5vkLQh9T+gePxDxFwbzhuzRCevLmQm5kobM7bEKiJleSaYiGjt19CN+AStyILN5w==;EndpointSuffix=core.windows.net";

    public OrderItemsReserver(ILogger<OrderItemsReserver> logger)
    {
        _logger = logger;
    }

    [Function("OrderItemsReserver")]
    public void Run([ServiceBusTrigger("orders-queue")] string myQueueItem)
    {
        try
        {
            string requestHead = "order-request-" + Guid.NewGuid().ToString("N") + ".json";

            UploadToBlobStorage(_storageConnectionString, "ishshopappblob", requestHead, myQueueItem);

            _logger.LogTrace("Order request generated and uploaded to Blob Storage.");
        }
        catch (Exception)
        {
            throw new ServiceBusException("Error generating order request and uploading to Blob Storage.");
        }
    }

    private static void UploadToBlobStorage(string connectionString, string containerName, string blobName, string data)
    {
        BlobRequestOptions blobOptions = new BlobRequestOptions()
        {
            RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(10), 3)
        };

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        blobClient.DefaultRequestOptions = blobOptions;

        CloudBlobContainer container = blobClient.GetContainerReference(containerName);
        container.CreateIfNotExistsAsync();

        CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
        blockBlob.UploadTextAsync(data);
    }

    public class ServiceBusException : Exception
    {
        public ServiceBusException(string message) : base(message)
        {
        }
    }
}
