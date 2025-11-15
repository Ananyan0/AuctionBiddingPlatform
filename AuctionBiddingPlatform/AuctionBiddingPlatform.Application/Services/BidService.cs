using AuctionBiddingPlatform.Core.DTOs.Bid;
using AuctionBiddingPlatform.Core.Entities;
using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using AutoMapper;

namespace AuctionBiddingPlatform.Application.Services;

public class BidService : IBidService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BidService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Bid>> GetUserBidsAsync(int userId)
    {
        var bids = await _unitOfWork.Bids.FindAsync(b => b.UserId == userId);

        return bids;
    }

    public async Task<BidResponseDto> PlaceBidAsync(int itemId, int userId, decimal amount)
    {
        var item = await _unitOfWork.AuctionItems.GetByIdWithBidsAsync(itemId);
        if (item == null)
            throw new InvalidOperationException($"Auction item with ID {itemId} not found.");


        item.PlaceBid(userId, amount);

        await _unitOfWork.SaveChangesAsync();

        var lastBid = item.Bids.Last();

        return _mapper.Map<BidResponseDto>(lastBid);
    }
}