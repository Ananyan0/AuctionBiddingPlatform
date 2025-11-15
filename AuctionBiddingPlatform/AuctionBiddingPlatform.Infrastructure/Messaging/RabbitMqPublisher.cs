using Auction.Contracts.Events;
using AuctionBiddingPlatform.Core.Interfaces.Messaging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AuctionBiddingPlatform.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly ConnectionFactory _factory;

    public RabbitMqPublisher(IConfiguration config)
    {
        _factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "localhost"
        };
    }

    public void PublishAuctionClosedAsync(AuctionClosedEvent ev)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "auction-closed",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var json = JsonSerializer.Serialize(ev);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(
            exchange: "",
            routingKey: "auction-closed",
            basicProperties: null,
            body: body
        );
    }
}