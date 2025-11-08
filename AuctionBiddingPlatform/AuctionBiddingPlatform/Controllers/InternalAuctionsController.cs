using AuctionBiddingPlatform.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuctionBiddingPlatform.Controllers;


[ApiController]
[Route("internal/auctions")]
public class InternalAuctionsController : ControllerBase
{
    private readonly IAuctionItemService _service;

    public InternalAuctionsController(IAuctionItemService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuction(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();

        return Ok(new
        {
            item.Id,
            item.Title,
            item.HighestBid,
            item.HighestBidUserId
        });
    }
}