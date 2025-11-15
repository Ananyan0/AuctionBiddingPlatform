using AuctionBiddingPlatform.Core.DTOs.AuctionItem;
using AuctionBiddingPlatform.Core.DTOs.Common;
using AuctionBiddingPlatform.Core.Entities;

namespace AuctionBiddingPlatform.Core.Interfaces.IServices;

public interface IAuctionItemService
{
    Task<IEnumerable<AuctionItem>> GetAllAsync();
    Task<AuctionItem?> GetByIdAsync(int id);
    Task<AuctionItem> CreateAsync(CreateAuctionItemRequestDto item);
    Task<IEnumerable<AuctionItem>> GetActiveAsync();
    Task CloseAuctionAsync(int id);

    Task<ICollection<AuctionItemListItemDto>> GetActivePagedAsync(AuctionItemFilterQuery query);

}
