namespace AuctionBiddingPlatform.Core.Interfaces.IRepositories;

public interface IUnitOfWork
{
    IAuctionItemRepository AuctionItems { get; }
    IBidRepository Bids { get; }

    Task<int> SaveChangesAsync();
}
