using Auction.Contracts.Events;
using AuctionBiddingPlatform.Core.DTOs.AuctionItem;
using AuctionBiddingPlatform.Core.Entities;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using AuctionBiddingPlatform.Core.Interfaces.Messaging;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace AuctionBiddingPlatform.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuctionItemsController : ControllerBase
{
    private readonly IAuctionItemService _auctionItemService;
    private readonly IMessagePublisher _publisher;
    private readonly IMapper _mapper;

    public AuctionItemsController(IAuctionItemService auctionItemService, IMapper mapper, IMessagePublisher publisher)
    {
        _auctionItemService = auctionItemService;
        _mapper = mapper;
        _publisher = publisher;
    }


    [HttpGet("active")]
    public async Task<IActionResult> GetActive([FromQuery] AuctionItemFilterQuery query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize is < 1 or > 100) query.PageSize = 10;

        var result = await _auctionItemService.GetActivePagedAsync(query);
        return Ok(result);
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var items = await _auctionItemService.GetAllAsync();
        var response = _mapper.Map<IEnumerable<AuctionItemResponseDto>>(items);
        return Ok(response);
    }

    //[HttpGet("active")]
    //[AllowAnonymous]
    //public async Task<IActionResult> GetActive()
    //{
    //    var items = await _auctionItemService.GetActiveAsync();
    //    var response = _mapper.Map<IEnumerable<AuctionItemResponseDto>>(items);
    //    return Ok(response);
    //}

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _auctionItemService.GetByIdAsync(id);
        if (item == null)
            return NotFound("Auction item not found.");

        var response = _mapper.Map<AuctionItemResponseDto>(item);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuctionItemRequestDto dto)
    {
        var item = _mapper.Map<AuctionItem>(dto);
        var created = await _auctionItemService.CreateAsync(item);
        var response = _mapper.Map<AuctionItemResponseDto>(created);
        return Ok(response);
    }

    [HttpPost("{id:int}/close")]
    public async Task<IActionResult> CloseAuction(int id)
    {
        await _auctionItemService.CloseAuctionAsync(id);


        await _publisher.PublishAuctionClosedAsync(new AuctionClosedEvent
        {
            AuctionItemId = id,
            ClosedAtUtc = DateTime.UtcNow
        });

        return Ok(new { Message = "Auction closed successfully." });
    }
}
