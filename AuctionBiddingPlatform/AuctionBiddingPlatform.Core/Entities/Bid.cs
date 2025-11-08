namespace AuctionBiddingPlatform.Core.Entities;

public class Bid
{
    public long Id { get; set; }
    public int AuctionItemId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PlacedAtUtc { get; set; }



    public AuctionItem AuctionItem { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
}
