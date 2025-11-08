using AuctionBiddingPlatform.Core.Enums;

namespace AuctionBiddingPlatform.Core.Entities;

public class AuctionItem
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Category Category { get; set; }

    public decimal StartingPrice { get; set; }
    public decimal? HighestBid { get; set; }
    public int? HighestBidUserId { get; set; }

    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public bool IsClosed { get; set; }

    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
}
