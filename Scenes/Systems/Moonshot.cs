using Godot;

namespace Spacelancer.Scenes.Systems;

public partial class Moonshot : Node2D
{
	public override void _Ready()
	{
		var station = GetNode<Stations.Station>("Silicon Mine");
		station.Id = "UA02_S01";
	}
	
}