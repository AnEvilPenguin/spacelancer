using Godot;

namespace Spacelancer.Components.Economy.Commodities;

public class Commodity
{
    public CommoditySize Size { get; }
    public string Name { get; }
    public int DefaultPrice { get; }
    public string Description { get; }
    
    public Texture2D Texture { get;  }

    public int GetQuantityFromVolume(int volume) =>
        Size switch
        {
            CommoditySize.Medium => volume / 5,
            _ => volume,
        };

    public Commodity(string name, int defaultPrice, string description, CommoditySize size = CommoditySize.Small, string textureName = "icon.svg")
    {
        Size = size;
        Name = name;
        DefaultPrice = defaultPrice;
        Description = description;
        
        // TODO pick a directory to use for these
        Texture = GD.Load<Texture2D>($"res://{textureName}");
    }
}