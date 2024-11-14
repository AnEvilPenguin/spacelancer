using Godot;
using Spacelancer.Controllers;
using Spacelancer.Scenes.Stations;
using Spacelancer.Universe;
using JumpGate = Spacelancer.Scenes.Transitions.JumpGate;
using Lane = Spacelancer.Scenes.Spacelane.Spacelane;

namespace Spacelancer.Scenes;

public partial class BaseSystem : Node2D
{
	public string Id;
	
	public override void _Ready()
	{
		var system = Global.Universe.GetSystem(Id);
		
		Name = system.Name;
		
		BuildStations(system);
		BuildJumpGates(system);
		BuildSpaceLanes(system);
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

	private void BuildSpaceLanes(SolarSystem system) =>
		system.GetSpaceLanes()
			.ForEach(laneConfig =>
			{
				var lane1 = Lane.GetNewInstance();
				var lane2 = Lane.GetNewInstance();
				
				lane1.Partner = lane2;
				lane2.Partner = lane1;
				
				lane1.Name = $"{laneConfig.Display1} => {laneConfig.Display2}";
				lane2.Name = $"{laneConfig.Display2} => {laneConfig.Display1}";

				lane1.Position = laneConfig.Location1.Position;
				lane2.Position = laneConfig.Location2.Position;
				
				lane1.RotationDegrees = laneConfig.Location1.RotationDegrees;
				lane2.RotationDegrees = laneConfig.Location2.RotationDegrees;
				
				AddChild(lane1);
				AddChild(lane2);
			});
}