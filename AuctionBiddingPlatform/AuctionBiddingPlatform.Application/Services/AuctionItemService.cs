using Auction.Contracts.Events;
using AuctionBiddingPlatform.Core.DTOs.AuctionItem;
using AuctionBiddingPlatform.Core.DTOs.Common;
using AuctionBiddingPlatform.Core.Entities;
using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using AuctionBiddingPlatform.Core.Interfaces.Messaging;
using AutoMapper;

namespace AuctionBiddingPlatform.Application.Services;

public class AuctionItemService : IAuctionItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _publisher;
    private readonly IMapper _mapper;

    public AuctionItemService(IUnitOfWork unitOfWork, IMapper mapper, IMessagePublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _publisher = publisher;
    }

    public async Task<IEnumerable<AuctionItem>> GetAllAsync()
        => await _unitOfWork.AuctionItems.GetAllAsync();

    public async Task<AuctionItem?> GetByIdAsync(int id)
    {
        var result = await _unitOfWork.AuctionItems.GetByIdAsync(id);

        return result ?? throw new Exception($"Auction item with {id} not found.");
    }

    public async Task<AuctionItem> CreateAsync(CreateAuctionItemRequestDto dto)
    {
        var entity = _mapper.Map<AuctionItem>(dto);

        entity.IsStartingPriceValid(dto.StartingPrice);
        entity.CreateAuction(dto.StartsAtUtc);

        await _unitOfWork.AuctionItems.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity;
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

        var evt = _mapper.Map<AuctionClosedEvent>(item);

        _publisher.PublishAuctionClosedAsync(evt);


        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ICollection<AuctionItemListItemDto>> GetActivePagedAsync(AuctionItemFilterQuery query)
    {
        var (entities, total) = await _unitOfWork.AuctionItems.GetActivePagedAsync(
            page: query.Page,
            pageSize: query.PageSize,
            category: query.Category,
            minPrice: query.MinPrice,
            maxPrice: query.MaxPrice,
            maxTimeRemainingMinutes: query.MaxTimeRemainingMinutes
        );

        var dtos = _mapper.Map<ICollection<AuctionItemListItemDto>>(entities);

        return dtos;
    }
}
