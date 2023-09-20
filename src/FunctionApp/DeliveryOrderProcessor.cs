using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp;

public class DeliveryOrderProcessor
{
    protected DeliveryOrderProcessor()
    {
    }

    [Function("DeliveryOrderProcessor")]
    public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, FunctionContext context)
    {
        var logger = context.GetLogger("CosmosDBWriter");

        try
        {
            var requestBody = req.ReadAsStringAsync().Result;
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(requestBody);

            CosmosClient cosmosClient = new CosmosClient("AccountEndpoint=https://ishwebappcosmosdb.documents.azure.com:443/;AccountKey=0muBlSm78yelYaXHTGjMB7hDAR9992ynDyQggWYI1jcwfbfOmkyJ45UmWCXuWlJgvkk4QvmlkpLqACDbSs7Wtg==;");

            var database = cosmosClient.GetDatabase("Orders");
            var container = database.GetContainer("Items");
            var response = container.CreateItemAsync(data).Result;

            var res = req.CreateResponse(HttpStatusCode.OK);
            res.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            res.WriteString("Order request generated and uploaded to Azure Cosmos DB.");
            return res;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating order request and uploading to Blob Storage.");

            var res = req.CreateResponse(HttpStatusCode.InternalServerError);
            return res;
        }
    }
}
