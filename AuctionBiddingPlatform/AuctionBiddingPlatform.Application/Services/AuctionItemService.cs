using AuctionBiddingPlatform.Core.DTOs.AuctionItem;
using AuctionBiddingPlatform.Core.DTOs.Common;
using AuctionBiddingPlatform.Core.Entities;
using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using AutoMapper;

namespace AuctionBiddingPlatform.Application.Services;

public class AuctionItemService : IAuctionItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuctionItemService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AuctionItem>> GetAllAsync()
        => await _unitOfWork.AuctionItems.GetAllAsync();

    public async Task<AuctionItem?> GetByIdAsync(int id)
        => await _unitOfWork.AuctionItems.GetByIdAsync(id);

    public async Task<AuctionItem> CreateAsync(AuctionItem item)
    {
        if(item.StartingPrice <= 0)
            throw new ArgumentException("Starting price must be greater than 0.");

        item.StartsAtUtc = DateTime.UtcNow;
        item.EndsAtUtc = DateTime.UtcNow.AddHours(24); 
        item.IsClosed = false;

        await _unitOfWork.AuctionItems.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item;
    }

    public async Task<IEnumerable<AuctionItem>> GetActiveAsync()
        => await _unitOfWork.AuctionItems.GetActiveAuctionsAsync();

    public async Task CloseAuctionAsync(int id)
    {
        var item = await _unitOfWork.AuctionItems.GetByIdAsync(id);
        if (item == null)
            throw new InvalidOperationException("Auction item not found.");

        item.IsClosed = true;
        _unitOfWork.AuctionItems.Update(item);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PagedResult<AuctionItemListItemDto>> GetActivePagedAsync(AuctionItemFilterQuery query)
    {
        var (entities, total) = await _unitOfWork.AuctionItems.GetActivePagedAsync(
            page: query.Page,
            pageSize: query.PageSize,
            category: query.Category,
            minPrice: query.MinPrice,
            maxPrice: query.MaxPrice,
            maxTimeRemainingMinutes: query.MaxTimeRemainingMinutes
        );

        var dtos = _mapper.Map<IReadOnlyList<AuctionItemListItemDto>>(entities);

        return new PagedResult<AuctionItemListItemDto>
        {
            Items = dtos,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };
    }
}
