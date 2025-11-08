using Auction.Contracts.DTOs;
using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Core.Interfaces.IServices;

namespace AuctionBiddingPlatform.Application.Services;

public sealed class AuctionQueryService : IAuctionQueryService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuctionQueryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AuctionWinnerSummaryDto?> GetWinnerSummaryAsync(int auctionItemId)
    {
        var item = await _unitOfWork.AuctionItems.GetByIdAsync(auctionItemId);
        if (item is null)
            return null;

        var bids = await _unitOfWork.Bids.FindAsync(b => b.AuctionItemId == auctionItemId);

        return new AuctionWinnerSummaryDto
        {
            AuctionItemId = item.Id,
            Title = item.Title,
            HighestBid = item.HighestBid,
            HighestBidUserId = item.HighestBidUserId,
            Bidders = bids
                .OrderByDescending(b => b.Amount)
                .Select(b => new BidderDto
                {
                    UserId = b.UserId,
                    Amount = b.Amount,
                    PlacedAtUtc = b.PlacedAtUtc
                })
                .ToList()
        };
    }
}
