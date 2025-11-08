using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Infrastructure.Data;

namespace AuctionBiddingPlatform.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuctionDbContext _context;
    public IAuctionItemRepository AuctionItems { get; }
    public IBidRepository Bids { get; }

    public UnitOfWork(
        AuctionDbContext context,
        IAuctionItemRepository auctionItems,
        IBidRepository bids)
    {
        _context = context;
        AuctionItems = auctionItems;
        Bids = bids;
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
