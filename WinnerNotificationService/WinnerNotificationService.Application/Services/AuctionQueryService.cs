using System.Net.Http.Json;
using WinnerNotificationService.Application.DTOs.Auction;
using WinnerNotificationService.Application.Interfaces;

namespace WinnerNotificationService.Application.Services;

public class AuctionQueryService : IAuctionQueryService
{
    private readonly HttpClient _httpClient;

    public AuctionQueryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(decimal? highestBid, int? highestUserId)> GetWinnerInfoAsync(int auctionId)
    {
        var result = await _httpClient.GetFromJsonAsync<AuctionResultDto>($"internal/auctions/{auctionId}");

        return (result?.HighestBid, result?.HighestBidUserId);
    }
}
