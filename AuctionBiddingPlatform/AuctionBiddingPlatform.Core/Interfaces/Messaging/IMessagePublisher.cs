using Auction.Contracts.Events;

namespace AuctionBiddingPlatform.Core.Interfaces.Messaging;

public interface IMessagePublisher
{
    void PublishAuctionClosedAsync(AuctionClosedEvent ev);
}
