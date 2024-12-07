using System.Collections.Generic;
using System.Linq;
using Godot;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Scenes.Stations.Components;

public partial class TrafficController : Node2D
{
	private List<Dock> _docks; 
	private List<Marker2D> _holdingLocations;
	private List<Marker2D> _navigationMarkers;
	
	public override void _Ready()
	{
		_docks = GetChildren()
			.OfType<Dock>()
			.ToList();

		_holdingLocations = GetChildren()
			.OfType<Marker2D>()
			.ToList();
		
		_navigationMarkers = GetNode("Markers")
			.GetChildren()
			.OfType<Marker2D>()
			.ToList();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// if there's nothing in the queue return
		// if there's a dock free, dequeue item and assign it to the free (closest?) dock
	}
	
	public Marker2D GetNearestMarker(Vector2 position) =>
		_navigationMarkers.Aggregate((acc, cur) =>
		{
			var dist1 = (position - cur.GlobalPosition).Length();
			var dist2 = (position - acc.GlobalPosition).Length();
			
			return dist1 < dist2 ? cur : acc;
		});

	// TODO make this work with new patterns.
	public AutomatedNavigation GetDockComputer() =>
		new StationDockingNavigation(GetParent<Station>());

	// when docking software requested add it to queue.
	// 
}