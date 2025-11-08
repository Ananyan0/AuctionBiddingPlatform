namespace WinnerNotificationService.Application.Interfaces;

public interface IAuctionQueryService
{
    Task<(decimal? highestBid, int? highestUserId)> GetWinnerInfoAsync(int auctionId);
}
