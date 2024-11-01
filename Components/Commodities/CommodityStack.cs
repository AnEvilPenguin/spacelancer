namespace Spacelancer.Components.Commodities;

public class CommodityStack
{
    public Commodity Commodity { get; private set; }
    public int Count { get; private set; }
    public int PurchasePrice { get; private set; }

    public CommodityStack(Commodity commodity, int count, int purchasePrice)
    {
       Commodity = commodity;
       Count = count;
       PurchasePrice = purchasePrice;
    }
    
    public int GetVolume() =>
        Commodity.Size switch
        {
            CommoditySize.Medium => Count * 5,
            _ => Count
        };
}