namespace AuctionBiddingPlatform.Core.Interfaces.IServices;

public interface IAuthService
{
    Task RegisterAsync(string userName, string email, string password);
    Task<string> LoginAsync(string userName, string password);
}
