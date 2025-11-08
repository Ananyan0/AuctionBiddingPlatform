using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using System.Text.Json;

namespace AuctionBiddingPlatform.Middlewares
{
    public class BiddingRulesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BiddingRulesMiddleware> _logger;

        public BiddingRulesMiddleware(RequestDelegate next, ILogger<BiddingRulesMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            // Apply only to POST /api/bids/{itemId}
            if (context.Request.Path.StartsWithSegments("/api/bids") &&
                context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var segments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);

                    // Expect route: /api/bids/{itemId}
                    if (segments is { Length: >= 3 } && int.TryParse(segments[2], out var itemId))
                    {
                        var auction = await unitOfWork.AuctionItems.GetByIdAsync(itemId);
                        if (auction == null)
                            throw new KeyNotFoundException("Auction item not found.");

                        if (auction.IsClosed)
                            throw new InvalidOperationException("Auction is already closed.");

                        if (auction.EndsAtUtc < DateTime.UtcNow)
                            throw new InvalidOperationException("Auction has expired.");

                        // ✅ Enable body buffering so controller can also read the body
                        context.Request.EnableBuffering();

                        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                        var body = await reader.ReadToEndAsync();
                        context.Request.Body.Position = 0; // Reset position for next middleware

                        if (!string.IsNullOrWhiteSpace(body))
                        {
                            try
                            {
                                var json = JsonSerializer.Deserialize<BidBody>(body, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                                if (json != null)
                                {
                                    var currentHighest = auction.HighestBid ?? auction.StartingPrice;
                                    var basis = auction.HighestBid.HasValue ? "current highest bid" : "starting price";

                                    if (json.Amount <= currentHighest)
                                        throw new ArgumentException(
                                            $"Bid must be higher than the {basis} ({currentHighest:F2}).");
                                }
                            }
                            catch (JsonException ex)
                            {
                                _logger.LogError(ex, "Failed to parse JSON bid body.");
                                throw new ArgumentException("Invalid JSON format in bid request.");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Request body cannot be empty.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Invalid bid route format.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Bidding validation failed.");
                    throw;
                }
            }

            // Continue the pipeline
            await _next(context);
        }

        private class BidBody
        {
            public decimal Amount { get; set; }
        }
    }
}
