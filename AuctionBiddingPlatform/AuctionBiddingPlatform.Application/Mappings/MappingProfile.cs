using AuctionBiddingPlatform.Core.DTOs.AuctionItem;
using AuctionBiddingPlatform.Core.DTOs.Bid;
using AuctionBiddingPlatform.Core.Entities;
using AutoMapper;

namespace AuctionBiddingPlatform.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auction Items
        CreateMap<CreateAuctionItemRequestDto, AuctionItem>();
        CreateMap<AuctionItem, AuctionItemResponseDto>();

        // Bids
        CreateMap<Bid, BidResponseDto>().ReverseMap();
        CreateMap<PlaceBidRequestDto, Bid>().ReverseMap();

        CreateMap<AuctionItem, AuctionItemListItemDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString()));
    }
}
