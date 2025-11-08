namespace AuctionBiddingPlatform.Core.DTOs.Bid;


public class BidResponseDto
{
    public int Id { get; set; }
    public int AuctionItemId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PlacedAtUtc { get; set; }
}