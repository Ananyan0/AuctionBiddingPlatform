using Auction.Contracts.Events;

namespace WinnerNotificationService.Application.Interfaces;

public interface INotificationHandler
{
    Task HandleAuctionClosedAsync(AuctionClosedEvent ev);
}
