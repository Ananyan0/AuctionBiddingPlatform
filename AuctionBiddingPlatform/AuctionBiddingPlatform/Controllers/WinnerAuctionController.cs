using AuctionBiddingPlatform.Core.DTOs.AuctionItem;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AuctionBiddingPlatform.Controllers
{

    [ApiController]
    [Route("internal/auctions")]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class WinnerAuctionController : ControllerBase
    {
        private readonly IAuctionItemService _service;
        private readonly IMapper _mapper;

        public WinnerAuctionController(IAuctionItemService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuction(int id)
        {
            var item = await _service.GetByIdAsync(id);
            
            return Ok(item);
        }
    }
}
