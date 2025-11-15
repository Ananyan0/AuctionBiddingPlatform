namespace AuctionBiddingPlatform.Core.DTOs.AuctionItem;


public class AuctionItemFilterQuery
{
    private int _page = 1;
    private int _pageSize = 10;



    public string? Category { get; set; }              
    public decimal? MinPrice { get; set; }            
    public decimal? MaxPrice { get; set; }
    public int? MaxTimeRemainingMinutes { get; set; } 



    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > 100 ? 10 : value;
    }
}