using Auction.Contracts.DTOs;

namespace AuctionBiddingPlatform.Core.Interfaces.IServices;

public interface IAuctionQueryService
{
    Task<AuctionWinnerSummaryDto?> GetWinnerSummaryAsync(int auctionItemId);
}
