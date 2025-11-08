using Auction.Contracts.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using WinnerNotificationService.Application.Interfaces;

namespace WinnerNotificationService.Infrastructure.Messaging
{
    public class AuctionClosedListener : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ConnectionFactory _factory;
        private readonly ILogger<AuctionClosedListener> _logger;

        public AuctionClosedListener(IConfiguration config, IServiceProvider services, ILogger<AuctionClosedListener> logger)
        {
            _services = services;
            _logger = logger;

            _factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:Host"] ?? "localhost",
                DispatchConsumersAsync = true
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("✅ WinnerNotificationService is now listening for auction-close events...");

            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("auction-closed", durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var ev = JsonSerializer.Deserialize<AuctionClosedEvent>(json);

                    _logger.LogInformation("📩 Received closed auction event for AuctionId={AuctionId}", ev!.AuctionItemId);

                    using var scope = _services.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<INotificationHandler>();

                    await handler.HandleAuctionClosedAsync(ev);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❗ Error occurred processing auction closed event.");
                }
            };

            channel.BasicConsume("auction-closed", autoAck: true, consumer);

            // ✅ This keeps the service running and prevents exit
            await Task.Delay(-1, stoppingToken);
        }
    }
}
