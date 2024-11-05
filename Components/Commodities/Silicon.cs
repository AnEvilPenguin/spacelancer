using Godot;

namespace Spacelancer.Components.Commodities;

public sealed class Silicon : Commodity
{
    public Silicon() {}

    public override string Name => "Silicon";
    public override CommoditySize Size => CommoditySize.Medium;
    public override int DefaultPrice => 40;
    public override string Description => "Silicon is used in the production of many electronic products.";
    public override Texture2D Texture => GD.Load<Texture2D>("res://icon.svg");
}