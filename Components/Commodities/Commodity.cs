namespace Spacelancer.Components.Commodities;

// TODO consider reworking this entirely.
// A Class for each commodity type

public sealed class Commodity
{
    public CommoditySize Size { get; private set; }
    public CommodityType Type { get; private set; }
    public int Quantity { get; private set; }
    public int PurchasePrice { get; private set; }

    public Commodity(CommoditySize size, CommodityType type, int quantity, int purchasePrice)
    {
        Size = size;
        Type = type;
        Quantity = quantity;
        PurchasePrice = purchasePrice;
    }
    
    public int GetVolume() =>
        Size switch
        {
            CommoditySize.Medium => Quantity * 5,
            _ => Quantity
        };
}