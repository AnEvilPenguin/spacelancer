using Godot;
using Spacelancer.Components.NPCs;

namespace Spacelancer.Scenes.Systems;

public partial class Sunrise : Node2D
{
	public override void _Ready()
	{
		var station1 = GetNode<Stations.Station>("Station1");
		var station2 = GetNode<Stations.Station>("Station2");
		
		var energyCell = Controllers.Global.Economy.GetCommodity("EnergyCell");
		var silicon = Controllers.Global.Economy.GetCommodity("Silicon");
		var microcontroller = Controllers.Global.Economy.GetCommodity("Microcontroller");
		
		station1.AddCommodityForSale(energyCell);
		station1.AddCommodityToBuy(microcontroller, 160);
		
		station2.AddCommodityToBuy(silicon, 135);
		station2.AddCommodityForSale(microcontroller, 116);

		var foo = new NonPlayerCharacter("Foo");
		foo.LoadDialog();
		
		station1.AddNpc(foo);
	}
}