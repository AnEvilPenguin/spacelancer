using Godot;
using System;
using Spacelancer.Components.Economy;

public partial class Moonshot : Node2D
{
	public override void _Ready()
	{
		var station = GetNode<Station>("Silicon Mine");
		
		station.AddCommodityForSale(Economy.Silicon, 45);
		station.AddCommodityToBuy(Economy.EnergyCell, 7);
	}
	
}
