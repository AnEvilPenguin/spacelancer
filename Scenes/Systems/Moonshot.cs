using Godot;
using Spacelancer.Controllers;
using Spacelancer.Scenes.Stations;
using Spacelancer.Universe;

namespace Spacelancer.Scenes.Systems;

public partial class Moonshot : Node2D
{
	public override void _Ready()
	{
		var system = Global.Universe.GetSystem("UA02");
		var stations = system.GetStations();
		
		foreach (var spaceStation in stations)
		{
			BuildStation(spaceStation);
		}
	}

	private void BuildStation(SpaceStation stationConfig)
	{
		var location = stationConfig.Location;
		
		var stationNode = Station.GetInstance(stationConfig.Type);
		
		stationNode.Position = location.Position;
		stationNode.Rotation = Mathf.DegToRad(location.RotationDegrees);
		
		var station = stationNode.GetNode<Station>("Station");
		
		station.Name = stationConfig.Name;
		station.Id = stationConfig.Id;
		
		AddChild(stationNode);
	}
}