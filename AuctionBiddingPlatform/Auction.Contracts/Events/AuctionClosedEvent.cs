namespace Auction.Contracts.Events;

public class AuctionClosedEvent
{
    public int AuctionItemId { get; init; }
    public DateTime ClosedAtUtc { get; init; } = DateTime.UtcNow;
}