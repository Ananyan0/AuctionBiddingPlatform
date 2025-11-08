using Auction.Contracts.Events;

namespace AuctionBiddingPlatform.Core.Interfaces.Messaging;

public interface IMessagePublisher
{
    Task PublishAuctionClosedAsync(AuctionClosedEvent ev);
}
