namespace AuctionBiddingPlatform.Core.Entities;

public class Bid
{
    public long Id { get; set; }
    public int AuctionItemId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PlacedAtUtc { get; set; }


    public AuctionItem AuctionItem { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public Bid(int auctionItemId, int userId, decimal amount)
    {
        AuctionItemId = auctionItemId;
        UserId = userId;
        Amount = amount;
        PlacedAtUtc = DateTime.UtcNow;
    }



    public void FindHigherBid(decimal amount)
    {
        var item = new AuctionItem();
        if (amount <= item.StartingPrice)
            throw new ArgumentException("Bid must be higher than the starting price.");

        if (item.HighestBid.HasValue)
        {
            if (amount <= item.HighestBid.Value)
                throw new ArgumentException("Bid must be higher than the current highest bid.");
        }
    }

    public static Bid Create(int auctionItemId, int userId, decimal amount)
           => new Bid(auctionItemId, userId, amount);
}
