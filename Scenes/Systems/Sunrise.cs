using Godot;
using Spacelancer.Components.Commodities;

public partial class Sunrise : Node2D
{
	public override void _Ready()
	{
		GetNode<Station>("Station2").OverrideCommodityPrice(CommodityType.EnergyCell, 5);
	}
}
