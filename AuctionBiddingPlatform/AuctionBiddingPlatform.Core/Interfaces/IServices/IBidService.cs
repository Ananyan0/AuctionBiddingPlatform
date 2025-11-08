using AuctionBiddingPlatform.Core.Entities;

namespace AuctionBiddingPlatform.Core.Interfaces.IServices;


public interface IBidService
{
    Task PlaceBidAsync(int itemId, int userId, decimal amount);
    Task<IEnumerable<Bid>> GetUserBidsAsync(int userId);
}