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
        if(!_dockables.TryGetValue(targetId, out var target))
            return null; // Better might be to throw an error?
        
        // TODO get all spacelanes and target.
        // Get all spacelanes that are closer than the target
            // if none just go direct
        
        // Get weighted distance for target
            // divide distance by speed?
        
        // Get weighted distance for lane
            // if weighted distance to target less than any lane, go direct to target
            
        // Get all lanes that are closer than the target
            // recheck distances if weighted distance to target less than any lane, go direct to target
        
        // Repeat until we have our best route
        
        // Build out list of computers and return
        
        return new SystemAutoNavigation(target);
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