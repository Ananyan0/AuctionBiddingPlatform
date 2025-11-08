using AuctionBiddingPlatform.Core.DTOs.Bid;
using AuctionBiddingPlatform.Core.Entities;
using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using AutoMapper;

namespace AuctionBiddingPlatform.Application.Services;

public class BidService : IBidService
{
    private readonly IUnitOfWork _unitOfWork;

    public BidService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Bid>> GetUserBidsAsync(int userId)
    {
        var bids = await _unitOfWork.Bids.FindAsync(b => b.UserId == userId);


        return bids;
    }
    public async Task PlaceBidAsync(int itemId, int userId, decimal amount)
    {
        var item = await _unitOfWork.AuctionItems.GetByIdAsync(itemId);
        if (item == null)
            throw new InvalidOperationException($"Auction item with ID {itemId} not found.");

        if (item.IsClosed || item.EndsAtUtc < DateTime.UtcNow)
            throw new InvalidOperationException("Auction is closed or expired.");

        var current = item.HighestBid ?? item.StartingPrice;
        if (amount <= current)
            throw new ArgumentException(
                $"Bid must be higher than {(item.HighestBid.HasValue ? "current highest bid" : "starting price")} ({current:F2}).");

        var bid = new Bid
        {
            AuctionItemId = itemId,
            UserId = userId,
            Amount = amount,
            PlacedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Bids.AddAsync(bid);

        item.HighestBid = amount;
        item.HighestBidUserId = userId;
        _unitOfWork.AuctionItems.Update(item);

        await _unitOfWork.SaveChangesAsync();
    }
}
