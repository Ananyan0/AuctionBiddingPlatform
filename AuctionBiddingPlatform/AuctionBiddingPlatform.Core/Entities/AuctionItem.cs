using AuctionBiddingPlatform.Core.Enums;
using AuctionBiddingPlatform.Core.Middlewares;

namespace AuctionBiddingPlatform.Core.Entities;

public class AuctionItem
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Category Category { get; set; }

    public decimal StartingPrice { get; set; }
    public decimal? HighestBid { get; set; }
    public int? HighestBidUserId { get; set; }

    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public bool IsClosed { get; set; }

    public ICollection<Bid> Bids { get; set; } = new List<Bid>();




    public void IsStartingPriceValid(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Starting price must be higher then 0");
    }

    public void IsDateValid(DateTime startDate)
    {
        if(startDate.AddMinutes(1) < DateTime.UtcNow)
            throw new ArgumentException("The date must be in the future");
    }


    public void IsClsoed(bool isClosed)
    {
        if (isClosed)
            throw new ArgumentException("The auction is closed");
    }

    private decimal GetCurrentHighestBid()
    {
        if (Bids == null || !Bids.Any())
        {
            return StartingPrice;
        }
        return Bids.Max(b => b.Amount);
    }


    public void RecalculateHighestBid()
    {
        if (Bids != null && Bids.Any())
        {
            var highestBid = Bids.OrderByDescending(b => b.Amount)
                                  .ThenByDescending(b => b.PlacedAtUtc)
                                  .First();
            HighestBid = highestBid.Amount;
            HighestBidUserId = highestBid.UserId;
        }
        else
        {
            HighestBid = null;
            HighestBidUserId = null;
        }
    }

    public void ValidateBid(decimal amount)
    {
        if (IsClosed)
            throw new DomainException("Auction is already closed.");

        if (EndsAtUtc < DateTime.UtcNow)
            throw new DomainException("Auction has expired.");

        var current = GetCurrentHighestBid();


        if (amount <= current)
            throw new DomainException(
                $"Bid must be higher than the current highest price ({current:F2}).");
    }


    public void PlaceBid(int userId, decimal amount)
    {
        var bid = Bid.Create(Id, userId, amount);
        Bids.Add(bid);


        HighestBid = amount;
        HighestBidUserId = userId;
    }


    public void CreateAuction(DateTime auctionStartDate)
    {
        IsDateValid(auctionStartDate);

        StartsAtUtc = auctionStartDate;
        EndsAtUtc = auctionStartDate.AddHours(24);
        IsClosed = false;
    }
}