using Godot;

namespace Spacelancer.Components.Commodities;

public abstract class Commodity
{
    public abstract CommoditySize Size { get; }
    public abstract string Name { get; }
    public abstract int DefaultPrice { get; }
    public abstract string Description { get; }
    
    public abstract Texture2D Texture { get;  }

    public int GetQuantityFromVolume(int volume) =>
        Size switch
        {
            CommoditySize.Medium => volume / 5,
            _ => volume,
        };
}