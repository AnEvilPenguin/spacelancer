using Godot;
using Spacelancer.Components.NPCs;

namespace Spacelancer.Scenes.Systems;

public partial class Sunrise : Node2D
{
	public override void _Ready()
	{
		var station1 = GetNode<Stations.Station>("Station1");
		station1.Id = "UA01_S01";
		
		var station2 = GetNode<Stations.Station>("Station2");
		station2.Id = "UA01_S02";

		var foo = new NonPlayerCharacter("Foo");
		foo.LoadDialog();
		
		station1.AddNpc(foo);
	}
}