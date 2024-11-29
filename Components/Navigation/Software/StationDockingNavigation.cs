using System;
using Godot;
using Serilog;
using Spacelancer.Scenes.Stations;

namespace Spacelancer.Components.Navigation.Software;

public class StationDockingNavigation : AutomatedNavigation
{
    private enum NavigationState
    {
        Travelling,
        Completed
    }

    public override string Name => $"{_hostStation.Name} Docking Navigation";
    public override NavigationSoftwareType Type => NavigationSoftwareType.Docking;

    private Station _hostStation;
    private NavigationState _state = NavigationState.Travelling;

    public StationDockingNavigation(Station station)
    {
        _hostStation = station;
    }

    public override event EventHandler<NavigationCompleteEventArgs> Complete;

    public override void DisruptTravel() =>
        RaiseEvent(Complete, new AbortedNavigationCompleteEventArgs());

    public override float GetRotation(float maxRotation, float currentAngle, Vector2 currentVelocity) =>
        currentVelocity.Angle();

    public override Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity) =>
        _state switch
        {
            NavigationState.Completed => ProcessCompletedVector(),
            _ => ProcessTravellingVector(currentPosition)
        };

    private Vector2 ProcessTravellingVector(Vector2 currentPosition)
    {
        var proposed = _hostStation.GlobalPosition - currentPosition;
        
        if (proposed.Length() < 5)
        {
            SetState(NavigationState.Completed);
            return Vector2.Zero;
        }
        
        return proposed.LimitLength(50);
    }

    private Vector2 ProcessCompletedVector()
    {
        RaiseEvent(Complete, new DockingNavigationCompleteEventArgs(_hostStation));
        
        return Vector2.Zero;
    }
    
    private void SetState(NavigationState newState)
    {
        Log.Debug("{Name} state change from {OldState} to {NewState}", Name, _state, newState);
        _state = newState;
    }
}