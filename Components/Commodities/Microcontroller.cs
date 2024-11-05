using Godot;

namespace Spacelancer.Components.Commodities;

public sealed class Microcontroller : Commodity
{
    public Microcontroller() {}


    public override CommoditySize Size => CommoditySize.Small;
    public override string Name => "Microcontroller";
    public override int DefaultPrice => 80;

    public override string Description =>
        "A small computer based on a single chip, used to build more complex electronic components";
    public override Texture2D Texture => GD.Load<Texture2D>("res://icon.svg");
}