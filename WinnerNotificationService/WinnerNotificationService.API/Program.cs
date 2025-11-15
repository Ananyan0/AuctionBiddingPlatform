using WinnerNotificationService.Application.Interfaces;
using WinnerNotificationService.Application.Services;
using WinnerNotificationService.Infrastructure.Messaging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// 1️⃣ Notification Handler
builder.Services.AddScoped<INotificationHandler, AuctionClosedNotificationHandler>();

// 2️⃣ HTTP client to query Auction Bidding API synchronously
builder.Services.AddHttpClient<IAuctionQueryService, AuctionQueryService>(client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("BiddingService__BaseUrl")
        ?? "http://localhost:5000/");


});

// 3️⃣ RabbitMQ Background Listener
builder.Services.AddHostedService<AuctionClosedListener>();

var host = builder.Build();
await host.RunAsync(); 