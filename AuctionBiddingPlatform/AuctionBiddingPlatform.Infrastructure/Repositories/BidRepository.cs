using AuctionBiddingPlatform.Core.Entities;
using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionBiddingPlatform.Infrastructure.Repositories;

public class BidRepository : BaseRepository<Bid>, IBidRepository
{
    public BidRepository(AuctionDbContext context) : base(context) { }

    public async Task<Bid?> GetHighestBidAsync(int itemId)
    {
        return await _dbSet
            .Where(b => b.AuctionItemId == itemId)
            .OrderByDescending(b => b.Amount)
            .FirstOrDefaultAsync();
    }
}
