using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

public class OrderJson
{
    public OrderJson(Order order)
    {
        Id = order.Id.ToString();
        ShipToAddress = order.ShipToAddress;
        Items = order.OrderItems.ToList();
        Total = order.Total();
    }

    [JsonPropertyName("id")]
    public String Id { get; private set; }

    public Address ShipToAddress { get; private set; }

    public List<OrderItem> Items { get; private set; }

    public decimal Total { get; private set; }
}
