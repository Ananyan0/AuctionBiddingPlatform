using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace AuctionBiddingPlatform.Core.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
}