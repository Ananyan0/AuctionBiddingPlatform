namespace Auction.Contracts.DTOs;

public class AuctionWinnerSummaryDto
{
    public int AuctionItemId { get; init; }
    public string Title { get; init; } = string.Empty;
    public decimal? HighestBid { get; init; }
    public int? HighestBidUserId { get; init; }
    public List<BidderDto> Bidders { get; init; } = new();
}
