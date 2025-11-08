using AuctionBiddingPlatform.Core.Entities;

namespace AuctionBiddingPlatform.Core.Interfaces.IRepositories;

public interface IBidRepository : IBaseRepository<Bid>
{
    Task<Bid?> GetHighestBidAsync(int itemId);
}
