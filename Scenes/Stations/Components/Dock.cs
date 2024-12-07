using Godot;
using Spacelancer.Components.Navigation;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Scenes.Stations.Components;

public partial class Dock : Node2D
{
	private StationDockingNavigation _current;
	
	public bool IsFree() =>
		_current == null;

	public bool AssignSoftwareSlot(StationDockingNavigation software)
	{
		if (!IsFree())
			return false;
		
		_current = software;

		_current.Complete += (object _, NavigationCompleteEventArgs _) =>
			_current = null;
		
		_current.SetDockingPort(this);
		
		return true;
	}
}