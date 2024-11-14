using System;
using Serilog;

namespace Spacelancer.Economy;

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

    public CommodityStack CombineStack(CommodityStack other)
    {
        if (Commodity.GetType() != other.Commodity.GetType())
        {
            Log.Error("Cannot combine stacks of different types - {Incoming} {Us}", other.Commodity.GetType(), Commodity.GetType());
            throw new ArgumentException("Cannot combine stacks of different types");
        }

        return new CommodityStack(Commodity, Count + other.Count, PurchasePrice + other.PurchasePrice);
    }
}