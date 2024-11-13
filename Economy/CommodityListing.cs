namespace Spacelancer.Economy;

public sealed class CommodityListing
{
    public int BuyingPrice
    {
        get
        {
            if (_buyingPrice < 1)
                return _commodity.DefaultPrice;
            
            return _buyingPrice;
        }
    }

    public int SellingPrice {         
        get
        {
            if (_sellingPrice < 1)
                return _commodity.DefaultPrice;
            
            return _sellingPrice;
        }
    }
    
    
    private readonly Commodity _commodity;
    
    private readonly int _buyingPrice;
    private readonly int _sellingPrice;
    
    public CommodityListing(Commodity commodity, int buyingPrice = 0, int sellingPrice = 0)
    {
        _commodity = commodity;
        
        _buyingPrice = buyingPrice;
        _sellingPrice = sellingPrice;
    }
}