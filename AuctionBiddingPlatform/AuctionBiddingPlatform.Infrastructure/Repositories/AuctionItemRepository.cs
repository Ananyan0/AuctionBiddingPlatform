using AuctionBiddingPlatform.Core.Entities;
using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionBiddingPlatform.Infrastructure.Repositories;

public class AuctionItemRepository : BaseRepository<AuctionItem>, IAuctionItemRepository
{
    private readonly AuctionDbContext _ctx;

    public AuctionItemRepository(AuctionDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }


    public async Task<IEnumerable<AuctionItem>> GetActiveAuctionsAsync()
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .Where(i => !i.IsClosed && i.StartsAtUtc <= now && i.EndsAtUtc >= now)
            .ToListAsync();
    }


    public IQueryable<AuctionItem> Query() => _ctx.AuctionItems.AsQueryable();

    public async Task<(IReadOnlyList<AuctionItem> Items, int TotalCount)> GetActivePagedAsync(
        int page, int pageSize,
        string? category,
        decimal? minPrice, decimal? maxPrice,
        int? maxTimeRemainingMinutes
        )
    {
        var now = DateTime.UtcNow;
        var q = _ctx.AuctionItems.AsNoTracking()
            .Where(a => !a.IsClosed && a.StartsAtUtc <= now && a.EndsAtUtc > now);

        if (!string.IsNullOrWhiteSpace(category))
        {
            q = q.Where(a => a.Category.ToString() == category);
        }

        if (minPrice.HasValue)
        {
            q = q.Where(a => (a.HighestBid ?? a.StartingPrice) >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            q = q.Where(a => (a.HighestBid ?? a.StartingPrice) <= maxPrice.Value);
        }

        if (maxTimeRemainingMinutes.HasValue)
        {
            var maxEndsAt = now.AddMinutes(maxTimeRemainingMinutes.Value);
            q = q.Where(a => a.EndsAtUtc <= maxEndsAt);
        }

        var total = await q.CountAsync();

        var items = await q
            .Skip((Math.Max(1, page) - 1) * Math.Max(1, pageSize))
            .Take(Math.Max(1, pageSize))
            .ToListAsync();

        return (items, total);
    }

    public async Task<AuctionItem?> GetByIdWithBidsAsync(int id)
    {
        var item =  await _context.AuctionItems
            .Include(a => a.Bids)
            .FirstOrDefaultAsync(a => a.Id == id);

        Console.WriteLine($"=== LOADED FROM DB ===");
        Console.WriteLine($"Item ID: {item?.Id}");
        Console.WriteLine($"Bids Count: {item?.Bids?.Count ?? 0}");
        Console.WriteLine($"HighestBid from DB: {item?.HighestBid}");

        item?.RecalculateHighestBid();

        Console.WriteLine($"HighestBid after recalc: {item?.HighestBid}");
        Console.WriteLine($"======================");

        return item;
    }
}