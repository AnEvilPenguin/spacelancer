using System;
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
    private class RouteCalculation
    {
        public HashSet<ulong> SeenIds = new();
        public Stack<Tuple<Vector2, SpaceLane>> Lanes = new();
        public float WeightedDistance;
        public float RemainingDistance;
        public Vector2 CurrentLocation;
        
        public RouteCalculation Clone() =>
            MemberwiseClone() as RouteCalculation;
    }
    
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

    private void BuildBestAutomatedRoute(Vector2 startPoint, Vector2 endPoint, Stack<AutomatedNavigation> output)
    {
        var weightedDistance = GetWeightedDistance(startPoint, endPoint, 150);

        var seed = new RouteCalculation() 
            { RemainingDistance = weightedDistance, CurrentLocation = startPoint };

        var routes = GetAutomatedRouteCalculation(seed, endPoint);
        
        if (routes.Count == 0)
            return;

        var bestRoute = routes.Aggregate((acc, cur) =>
        {
            if (acc.WeightedDistance < cur.WeightedDistance)
                return acc;
            
            return cur;
        });

        foreach (var tuple in bestRoute.Lanes)
        {
            var entrance = tuple.Item2.GetNearestEntrance(tuple.Item1);
            
            // LIFO
            output.Push(entrance.GetDockComputer());
            output.Push(new SystemAutoNavigation(entrance));
        }
    }

    private List<RouteCalculation> GetAutomatedRouteCalculation(RouteCalculation seed, Vector2 endPoint)
    {
        List<RouteCalculation> output = new ();

        var start = seed.CurrentLocation;
        var remaining = seed.RemainingDistance;
        var seen = seed.SeenIds;
        
        var dist = start.DistanceTo(endPoint);
        var rawRemaining = remaining * 150;
        
        var possibleLanes = GetLanesWithinRange(start, dist < rawRemaining ? dist : rawRemaining)
            .Where(l => !seen.Contains(l.GetInstanceId()));

        foreach (var lane in possibleLanes)
        {
            var tuple = lane.GetNavigationPositions(seed.CurrentLocation);
            
            var weightedStart = GetWeightedDistance(start, tuple.Item1, 150);
            var weightedMiddle = GetWeightedDistance(tuple.Item1, tuple.Item2, 2500);
            var weighedEnd = GetWeightedDistance(tuple.Item2, endPoint, 150);

            var total = weightedStart + weightedMiddle + weighedEnd;

            var inRange = total < remaining;
            
            if (inRange)
            {
                var clone = seed.Clone();
                clone.CurrentLocation = tuple.Item2;
                clone.WeightedDistance += weightedStart + weightedMiddle;
                clone.RemainingDistance -= weightedStart + weightedMiddle;
                clone.Lanes.Push(new Tuple<Vector2, SpaceLane>(seed.CurrentLocation, lane));
                clone.SeenIds.Add(lane.GetInstanceId());
                
                output.Add(clone);
                
                output.AddRange(GetAutomatedRouteCalculation(clone, endPoint));
            }
        }

        return output;
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