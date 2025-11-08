using Auction.Contracts.Events;
using WinnerNotificationService.Application.Interfaces;

namespace WinnerNotificationService.Application.Services;

public class AuctionClosedNotificationHandler : INotificationHandler
{
    private readonly IAuctionQueryService _query;

    public AuctionClosedNotificationHandler(IAuctionQueryService query)
    {
        _query = query;
    }

    public async Task HandleAuctionClosedAsync(AuctionClosedEvent ev)
    {
        var (highestBid, userId) = await _query.GetWinnerInfoAsync(ev.AuctionItemId);

        if (userId == null)
        {
            Console.WriteLine($"[WinnerService] Auction {ev.AuctionItemId} ended with no bids.");
            return;
        }

        Console.WriteLine($"🎉 Auction {ev.AuctionItemId} Winner: User #{userId} with bid {highestBid} AMD");
    }
}