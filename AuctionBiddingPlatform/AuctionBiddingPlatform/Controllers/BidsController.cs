using AuctionBiddingPlatform.Core.DTOs.Bid;
using AuctionBiddingPlatform.Core.Entities;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionBiddingPlatform.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BidsController : ControllerBase
{
    private readonly IBidService _bidService;
    private readonly IMapper _mapper;

    public BidsController(IBidService bidService, IMapper mapper)
    {
        _bidService = bidService;
        _mapper = mapper;
    }


    [Authorize]
    [HttpPost("{itemId}")]
    public async Task<IActionResult> PlaceBid(int itemId, [FromBody] PlaceBidRequestDto dto)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        var result = await _bidService.PlaceBidAsync(itemId, userId, dto.Amount);

        return Ok(result);
    }


    [Authorize]
    [HttpGet("my-bids/{userId}")]
    public async Task<IEnumerable<Bid>> GetUserBids(int userId)
    {
        var bids = await _bidService.GetUserBidsAsync(userId);

        return bids;
    }
}