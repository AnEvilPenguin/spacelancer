using Godot;
using Spacelancer.Controllers;
using Spacelancer.Scenes.Stations;
using Spacelancer.Universe;
using JumpGate = Spacelancer.Scenes.Transitions.JumpGate;

namespace Spacelancer.Scenes.Systems;

public partial class Moonshot : Node2D
{
	public override void _Ready()
	{
		var system = Global.Universe.GetSystem("UA02");
		
		BuildStations(system);
		BuildJumpGates(system);
	}
	
	private void BuildStations(SolarSystem system)
	{
		var stations = system.GetStations();
		
		foreach (var spaceStation in stations)
		{
			BuildStation(spaceStation);
		}
	}

	private void BuildStation(SpaceStation stationConfig)
	{
		var location = stationConfig.Location;
		
		var stationNode = Station.GetNewInstance(stationConfig.Type);
		
		stationNode.Position = location.Position;
		stationNode.Rotation = Mathf.DegToRad(location.RotationDegrees);
		
		var station = stationNode.GetNode<Station>("Station");
		
		station.Name = stationConfig.Name;
		station.Id = stationConfig.Id;
		
		AddChild(stationNode);
	}
	
	private void BuildJumpGates(SolarSystem system) =>
		system.GetJumpGates()
			.ForEach(gateConfig =>
			{
				var gate = JumpGate.GetNewInstance();
				var location = gateConfig.Location;
				
				gate.Name = gateConfig.Name;
				gate.Position = location.Position;
				gate.Rotation = Mathf.DegToRad(location.RotationDegrees);
				
				AddChild(gate);
			});
}