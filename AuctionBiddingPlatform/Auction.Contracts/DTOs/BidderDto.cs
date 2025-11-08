namespace Auction.Contracts.DTOs;

public class BidderDto
{
    public int UserId { get; init; }
    public decimal Amount { get; init; }
    public DateTime PlacedAtUtc { get; init; }
}
