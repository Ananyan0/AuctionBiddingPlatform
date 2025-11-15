namespace AuctionBiddingPlatform.Core.DTOs.AuctionItem;

public class CreateAuctionItemRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal StartingPrice { get; set; }
    public DateTime StartsAtUtc { get; set; }
}