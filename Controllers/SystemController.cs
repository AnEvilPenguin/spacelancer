using System.Collections.Generic;
using System.Linq;
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
    
    private readonly List<SpaceLane> _spaceLanes = new();

    public Stack<AutomatedNavigation> GetAutomatedRoute(Vector2 startPoint, string targetId)
    {
        Stack<AutomatedNavigation> output = new ();
        
        if(!_dockables.TryGetValue(targetId, out var target))
            return null; // Better might be to throw an error?
        
        output.Push(target.GetDockComputer());
        output.Push(new SystemAutoNavigation(target));
        
        BuildBestAutomatedRoute(startPoint, target.GlobalPosition, output);
        
        return output;
    }
    
    // 150 current max speed. We might want to move this to a constant or otherwise find a way of dealing with this
    // 2500 for lane
    
    // Get weighted distance for lane
    // if weighted distance to target less than any lane, go direct to target
            
    // Get all lanes that are closer than the target
    // recheck distances if weighted distance to target less than any lane, go direct to target
        
    // Repeat until we have our best route
        
    // Build out list of computers and return

    private void BuildBestAutomatedRoute(Vector2 startPoint, Vector2 endPoint, Stack<AutomatedNavigation> output)
    {
        var distance = startPoint.DistanceTo(endPoint);

        var possibleLanes = GetLanesWithinRange(startPoint, distance);

        var weightedDistance = GetWeightedDistance(startPoint, endPoint, 150);

        // TODO consider a class for making a private class to make holding these results easier?
        var quickerLanes = possibleLanes.Where(l =>
        {
            var tuple = l.GetNavigationPositions(startPoint);

            // Can we get the lane to give us some of this?
            // Or just move some of the specific speeds to constants or the like.
            var weightedStart = GetWeightedDistance(startPoint, tuple.Item1, 150);
            var weightedMiddle = GetWeightedDistance(tuple.Item1, tuple.Item2, 2500);
            var weighedEnd = GetWeightedDistance(tuple.Item2, endPoint, 150);

            var total = weightedStart + weightedMiddle + weighedEnd;

            return total < weightedDistance;
        });
        
        var first = quickerLanes.FirstOrDefault();
        
        if (first == null)
            return;
        
        var entrance = first.GetNearestEntrance(startPoint);
        
        // LIFO
        output.Push(entrance.GetDockComputer());
        output.Push(new SystemAutoNavigation(entrance));
    }

    public AutomatedNavigation CalculateBestRoute(Vector2 startPoint, string targetId)
    {
        if(!_dockables.TryGetValue(targetId, out var target))
            return null; // Better might be to throw an error?
        
        GetAutomatedRoute(startPoint, targetId);

        var distanceToTarget = startPoint.DistanceTo(target.GlobalPosition);

        var possibleLanes = GetLanesWithinRange(startPoint, distanceToTarget);
        
        if (!possibleLanes.Any())
            return new SystemAutoNavigation(target);


        
        return new SystemAutoNavigation(target);
    }

    private void ProcessLists(BaseSystem system)
    {
        _spaceLanes.Clear();
        _dockables.Clear();

        var children = system.GetChildren();

        foreach (var child in children)
        {
            if (child is JumpGate jumpGate)
                _dockables.Add(jumpGate.Id, jumpGate);
            
            if (child is SpaceLane spaceLane)
                _spaceLanes.Add(spaceLane);

            if (child is Station station)
                _dockables.Add(station.Id, station);
        }
    }
    
    private float GetWeightedDistance(Vector2 startPoint, Vector2 endPoint, float speed) =>
        startPoint.DistanceTo(endPoint) / speed;

    private IEnumerable<SpaceLane> GetLanesWithinRange(Vector2 startPoint, float maxDistance) =>
        _spaceLanes.Where(l =>
        {
            var entrancePosition = l.GetNavigationPositions(startPoint).Item1;
            var distance = startPoint.DistanceTo(entrancePosition);
            return distance < maxDistance;
        });
}