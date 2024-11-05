using Godot;

namespace Spacelancer.Components.Commodities;

public sealed class EnergyCell : Commodity
{
    public EnergyCell() {}

    public override CommoditySize Size => CommoditySize.Small;
    public override string Name => "Energy Cell";
    public override int DefaultPrice => 5;
    public override string Description => "An energy cell is the most prolific thing in the universe.";
    
    public override Texture2D Texture => GD.Load<Texture2D>("res://icon.svg");
}