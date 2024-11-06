using System;
using System.Collections.Generic;
using Godot;
using Serilog;
using Spacelancer.Components.Economy;
using Spacelancer.Components.NPCs;
using Spacelancer.Util;

public partial class Sunrise : Node2D
{
	public override void _Ready()
	{
		var station1 = GetNode<Station>("Station1");
		var station2 = GetNode<Station>("Station2");
		
		station1.AddCommodityForSale(Economy.EnergyCell);
		station1.AddCommodityToBuy(Economy.Microcontroller, 160);
		
		station2.AddCommodityToBuy(Economy.Silicon, 135);
		station2.AddCommodityForSale(Economy.Microcontroller, 116);

		var foo = new NonPlayerCharacter("Foo");
		foo.LoadDialog();
		
		station1.AddNpc(foo);
	}
}
