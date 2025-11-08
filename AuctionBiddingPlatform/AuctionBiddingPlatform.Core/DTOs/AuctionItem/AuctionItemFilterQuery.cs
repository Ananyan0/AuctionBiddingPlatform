namespace AuctionBiddingPlatform.Core.DTOs.AuctionItem;


public class AuctionItemFilterQuery
{
    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Filtering
    public string? Category { get; set; }              
    public decimal? MinPrice { get; set; }            
    public decimal? MaxPrice { get; set; }
    public int? MaxTimeRemainingMinutes { get; set; } 
}