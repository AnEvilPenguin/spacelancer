using System.Collections.Generic;
using Godot;
using Spacelancer.Components.Navigation;
using Spacelancer.Components.Navigation.Software;
using Spacelancer.Scenes.SolarSystems;
using Spacelancer.Scenes.Stations;
using Spacelancer.Scenes.Transitions;

namespace Spacelancer.Controllers;

public class SystemController
{
    public BaseSystem CurrentSystem { 
        get => _currentSystem;
        set
        {
            _currentSystem = value;
            ProcessLists(_currentSystem);
        }
    }
    
    private BaseSystem _currentSystem;
    
    private readonly Dictionary<string, IDockable> _dockables  = new ();
    
    private readonly List<JumpGate> _jumpGates = new ();
    private readonly List<SpaceLane> _spaceLanes = new();
    private readonly List<Station> _stations = new ();

    public AutomatedNavigation CalculateBestRoute(Vector2 startPoint, string targetId)
    {
        if(!_dockables.TryGetValue(targetId, out var dockable))
            return null; // Better might be to throw an error?
        
        return new SystemAutoNavigation(dockable);
    }

    private void ProcessLists(BaseSystem system)
    {
        _jumpGates.Clear();
        _spaceLanes.Clear();
        _stations.Clear();
        _dockables.Clear();

        var children = system.GetChildren();

        foreach (var child in children)
        {
            if (child is JumpGate jumpGate)
            {
                _jumpGates.Add(jumpGate);
                _dockables.Add(jumpGate.Id, jumpGate);
            }
                
            
            if (child is SpaceLane spaceLane)
                _spaceLanes.Add(spaceLane);

            if (child is Station station)
            {
                _stations.Add(station);
                _dockables.Add(station.Id, station);
            }
        }
    }
}