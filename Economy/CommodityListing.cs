namespace Spacelancer.Economy;

public sealed class CommodityListing
{
    public int Price
    {
        get
        {
            if (_price < 1)
                return Commodity.DefaultPrice;
            
            return _price;
        }
    }

    public readonly Commodity Commodity;
    public readonly TransactionType Type;
    
    
    private readonly int _price;

    public CommodityListing(Commodity commodity, TransactionType type, int price)
    {
        Commodity = commodity;
        
        Type = type;
        _price = price;
    }
}