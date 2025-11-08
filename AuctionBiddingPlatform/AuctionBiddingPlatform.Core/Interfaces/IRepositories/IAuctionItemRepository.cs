using AuctionBiddingPlatform.Core.Entities;

namespace AuctionBiddingPlatform.Core.Interfaces.IRepositories;

public interface IAuctionItemRepository : IBaseRepository<AuctionItem>
{
    Task<IEnumerable<AuctionItem>> GetActiveAuctionsAsync();

    IQueryable<AuctionItem> Query(); 
    Task<(IReadOnlyList<AuctionItem> Items, int TotalCount)> GetActivePagedAsync(
        int page, int pageSize,
        string? category,
        decimal? minPrice, decimal? maxPrice,
        int? maxTimeRemainingMinutes
        );
}