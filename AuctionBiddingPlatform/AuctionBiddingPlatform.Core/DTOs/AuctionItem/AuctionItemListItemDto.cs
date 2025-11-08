namespace AuctionBiddingPlatform.Core.DTOs.AuctionItem;

public class AuctionItemListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = ""; // string name of enum for convenience
    public decimal StartingPrice { get; set; }
    public decimal? HighestBid { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public bool IsClosed { get; set; }
}
