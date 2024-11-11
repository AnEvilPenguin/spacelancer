using Godot;
using System;
using Spacelancer.Components.Commodities;
using Spacelancer.Components.Economy;

public partial class Moonshot : Node2D
{
	public override void _Ready()
	{
		var station = GetNode<Station>("Silicon Mine");

		var energyCell = Global.Economy.GetCommodity(CommodityType.EnergyCell);
		var silicon = Global.Economy.GetCommodity(CommodityType.Silicon);
		
		station.AddCommodityForSale(silicon, 45);
		station.AddCommodityToBuy(energyCell, 7);
	}
	
}
