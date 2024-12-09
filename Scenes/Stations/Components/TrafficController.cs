using System.Collections.Generic;
using System.Linq;
using Godot;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Scenes.Stations.Components;

public partial class TrafficController : Node2D
{
	private Station _parent;
	
	private List<Dock> _docks; 
	private List<Marker2D> _holdingLocations;
	private List<Marker2D> _navigationMarkers;
	
	private Queue<StationDockingNavigation> _dockingQueue = new();
	
	public override void _Ready()
	{
		_parent = GetParent<Station>();
		
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
		bool shouldContinue = _dockingQueue.Count > 0;

		while (shouldContinue)
		{
			// if there's a dock free, dequeue item and assign it to the free (closest?) dock
			shouldContinue = ProcessQueue() && _dockingQueue.Count > 0;
		} 
	}
	
	public Marker2D GetNearestMarker(Vector2 position) =>
		_navigationMarkers.Aggregate((acc, cur) =>
		{
			var dist1 = (position - cur.GlobalPosition).Length();
			var dist2 = (position - acc.GlobalPosition).Length();
			
			return dist1 < dist2 ? cur : acc;
		});
	
	public AutomatedNavigation GetDockComputer(Vector2 _)
	{
		var index = GD.Randi() % _holdingLocations.Count;
		var location = _holdingLocations[(int)index].GlobalPosition;
		
		var software = new StationDockingNavigation(_parent, location);
		
		_dockingQueue.Enqueue(software);

		return software;
	}

	private bool ProcessQueue()
	{
		var freeDock = _docks.FirstOrDefault(d => d.IsFree());
		
		if (freeDock == null)
			return false;
		
		if (!_dockingQueue.TryDequeue(out var software))
			return false;

		if (!freeDock.AssignSoftwareSlot(software))
		{
			_dockingQueue.Enqueue(software);
			return false;
		}
		
		return true;
	}
}