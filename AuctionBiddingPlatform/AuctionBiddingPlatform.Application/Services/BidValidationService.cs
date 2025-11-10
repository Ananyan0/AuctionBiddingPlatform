using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Core.Interfaces.IServices;

namespace AuctionBiddingPlatform.Application.Services;

public class BidValidationService : IBidValidationService
{
    private readonly IUnitOfWork _unitOfWork;

    public BidValidationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task ValidateAsync(int itemId, decimal amount)
    {
        var auction = await _unitOfWork.AuctionItems.GetByIdAsync(itemId);

        if (auction == null)
            throw new KeyNotFoundException("Auction item not found.");

        auction.ValidateBid(amount);
    }
}