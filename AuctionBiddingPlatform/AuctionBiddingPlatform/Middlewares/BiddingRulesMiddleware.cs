using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AuctionBiddingPlatform.Middlewares;

public class BiddingRulesMiddleware
{
    private readonly RequestDelegate _next;

    public BiddingRulesMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IBidValidationService validator)
    {
        if (context.Request.Path.StartsWithSegments("/api/bids") &&
               context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            var segments = context.Request.Path.Value!.Split('/', StringSplitOptions.RemoveEmptyEntries);


            if (segments.Length >= 3 && int.TryParse(segments[2], out var itemId))
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                var data = JsonSerializer.Deserialize<BidBody>(body);
                if (data == null)
                    throw new ArgumentException("Invalid request body.");

                await validator.ValidateAsync(itemId, data.Amount);
            }
        }

        await _next(context);
    }

    private class BidBody
    {
        public decimal Amount { get; set; }
    }
}
