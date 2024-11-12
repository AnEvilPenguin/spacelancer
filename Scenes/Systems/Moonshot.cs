using Godot;

namespace Spacelancer.Scenes.Systems;

public partial class Moonshot : Node2D
{
	public override void _Ready()
	{
		var station = GetNode<Stations.Station>("Silicon Mine");

		var energyCell = Controllers.Global.Economy.GetCommodity("EnergyCell");
		var silicon = Controllers.Global.Economy.GetCommodity("Silicon");
		
		station.AddCommodityForSale(silicon, 45);
		station.AddCommodityToBuy(energyCell, 7);
	}
	
}