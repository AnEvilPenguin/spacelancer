namespace Spacelancer.Components.Commodities;

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
}