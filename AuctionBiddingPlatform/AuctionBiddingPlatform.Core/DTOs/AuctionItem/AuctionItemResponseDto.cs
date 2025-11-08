namespace AuctionBiddingPlatform.Core.DTOs.AuctionItem;

public class AuctionItemResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal StartingPrice { get; set; }
    public decimal? HighestBid { get; set; }
    public bool IsClosed { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
}