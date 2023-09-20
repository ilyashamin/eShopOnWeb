using System.Text;

namespace Microsoft.eShopWeb.Web.Clients;

public class OrderApiClient
{
    private readonly string functionUrl;

    public OrderApiClient(string functionUrl)
    {
        this.functionUrl = functionUrl;
    }

    public async Task<string> ReserveOrderItemsAsync(string orderDetailsJson, string orderHeader = "")
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, functionUrl);

                if (!String.IsNullOrWhiteSpace(orderHeader))
                {
                    request.Headers.Add("Order-Header", orderHeader);
                }

                request.Content = new StringContent(orderDetailsJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return "Error: " + response.StatusCode.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            return "Exception: " + ex.Message;
        }
    }
}
