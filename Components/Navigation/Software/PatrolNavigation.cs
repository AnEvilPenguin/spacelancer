using System;
using System.Collections.Generic;
using Godot;
using Serilog;

namespace Spacelancer.Components.Navigation.Software;

public class PatrolNavigation : AutomatedNavigation
{
    private enum PatrolState
    {
        Patrol,
        Restarting
    }
    
    public override event EventHandler<NavigationCompleteEventArgs> Complete;
    public override void DisruptTravel() =>
        RaiseEvent(Complete, new AbortedNavigationCompleteEventArgs());

    public override string Name => $"{_name} Patrol";
    public override NavigationSoftwareType Type => NavigationSoftwareType.Navigation;
    
    private readonly string _name;
    private readonly List<Vector2> _route;
    private readonly bool _reverseRouteOnComplete;
    
    private PatrolState _state = PatrolState.Patrol;
    private int _currentRouteIndex = 0;

    public PatrolNavigation(string name, List<Vector2> route, bool reverseRouteOnComplete = false)
    {
        _name = name;
        _route = route;
        _reverseRouteOnComplete = reverseRouteOnComplete;
    }
        
    public override float GetRotation(float maxRotation, float currentAngle, Vector2 currentVelocity) => 
        currentVelocity.Angle();

    public override Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity) =>
        _state switch
        {
            PatrolState.Patrol => ProcessPatrol(maxSpeed, currentPosition, currentVelocity),
            _ => ProcessRestart(currentVelocity)
        };

    private Vector2 ProcessPatrol(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity)
    {
        if (_currentRouteIndex >= _route.Count)
        {
            SetState(PatrolState.Restarting);
            return currentVelocity;
        }
        
        var target = _route[_currentRouteIndex];
        var distanceToTarget = currentPosition.DistanceTo(target);
        
        Vector2 proposed = target - currentPosition;

        if (distanceToTarget < 50)
        {
            _currentRouteIndex++;
            return (proposed.LimitLength(25) + currentVelocity).LimitLength(50);
        }
        
        if (distanceToTarget < maxSpeed)
            return (proposed.LimitLength(maxSpeed / 2) + currentVelocity).LimitLength(maxSpeed);
        
        return (proposed + currentVelocity).LimitLength(maxSpeed);
        
    }
    
    private Vector2 ProcessRestart(Vector2 currentVelocity)
    {
        _currentRouteIndex = 0;
        
        if (_reverseRouteOnComplete)
            _route.Reverse();
        
        SetState(PatrolState.Patrol);
        
        return currentVelocity;
    }
    
    private void SetState(PatrolState newState)
    {
        Log.Debug("{Name} state change from {OldState} to {NewState}", Name, _state, newState);
        _state = newState;
    }
}