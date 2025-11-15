using AuctionBiddingPlatform.Core.DTOs.AuctionItem;
using AuctionBiddingPlatform.Core.Entities;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using AuctionBiddingPlatform.Core.Interfaces.Messaging;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
      
        var result = await _auctionItemService.GetActivePagedAsync(query);
        return Ok(result);
    }


    [HttpGet("Get all auctions")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var items = await _auctionItemService.GetAllAsync();

        var response = _mapper.Map<IEnumerable<AuctionItemResponseDto>>(items);

        return Ok(response);
    }


    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _auctionItemService.GetByIdAsync(id);

        var response = _mapper.Map<AuctionItemResponseDto>(item);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuctionItemRequestDto dto)
    {
        var created = await _auctionItemService.CreateAsync(dto);
     
        var response = _mapper.Map<AuctionItemResponseDto>(created);
        
        return Ok(response);
    }

    [HttpPost("{id:int}/close")]
    public async Task<IActionResult> CloseAuction(int id)
    {
        await _auctionItemService.CloseAuctionAsync(id);

        return Ok(new { Message = "Auction closed successfully." });
    }
}