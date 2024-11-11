using System;
using System.Collections.Generic;
using Godot;
using Serilog;
using Spacelancer.Components.Commodities;
using Spacelancer.Components.Economy;
using Spacelancer.Components.NPCs;
using Spacelancer.Util;

public partial class Sunrise : Node2D
{
	public override void _Ready()
	{
		var station1 = GetNode<Station>("Station1");
		var station2 = GetNode<Station>("Station2");
		
		var energyCell = Global.Economy.GetCommodity(CommodityType.EnergyCell);
		var silicon = Global.Economy.GetCommodity(CommodityType.Silicon);
		var microcontroller = Global.Economy.GetCommodity(CommodityType.Microcontroller);
		
		station1.AddCommodityForSale(energyCell);
		station1.AddCommodityToBuy(microcontroller, 160);
		
		station2.AddCommodityToBuy(silicon, 135);
		station2.AddCommodityForSale(microcontroller, 116);

		var foo = new NonPlayerCharacter("Foo");
		foo.LoadDialog();
		
		station1.AddNpc(foo);
	}
}
