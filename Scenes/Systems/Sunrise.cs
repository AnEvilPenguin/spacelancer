using Godot;
using Spacelancer.Components.Commodities;
using Spacelancer.Components.Economy;

public partial class Sunrise : Node2D
{
	public override void _Ready()
	{
		var station1 = GetNode<Station>("Station1");
		var station2 = GetNode<Station>("Station2");
		
		station1.AddCommodityForSale(Economy.EnergyCell);
		station2.AddCommodityToBuy(Economy.EnergyCell, 8);
	}
}
