using System;
using Godot;
using Serilog;
using Spacelancer.Scenes.Stations;
using Spacelancer.Scenes.Stations.Components;

namespace Spacelancer.Components.Navigation.Software;

public class StationDockingNavigation : AutomatedNavigation
{
    private enum NavigationState
    {
        Holding,
        Docking,
        Completed
    }

    public override string Name => $"{_hostStation.Name} Docking Navigation";
    public override NavigationSoftwareType Type => NavigationSoftwareType.Docking;

    private Station _hostStation;
    private NavigationState _state = NavigationState.Holding;
    
    private Dock _assignedPort;
    private readonly Vector2 _holdingPosition;

    public StationDockingNavigation(Station station, Vector2 holdingPosition)
    {
        _hostStation = station;
        _holdingPosition = holdingPosition;
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
            NavigationState.Docking => ProcessDockingVector(currentPosition),
            NavigationState.Holding => ProcessHoldingVector(maxSpeed, currentPosition),
            _ => Vector2.Zero,
        };

    public void SetDockingPort(Dock port)
    {
        _assignedPort = port;
        SetState(NavigationState.Docking);
    }
        

    private Vector2 ProcessDockingVector(Vector2 currentPosition)
    {
        var proposed = _assignedPort.GlobalPosition - currentPosition;
        
        if (proposed.Length() < 5)
        {
            SetState(NavigationState.Completed);
            return Vector2.Zero;
        }
        
        return proposed.LimitLength(50);
    }

    private Vector2 ProcessHoldingVector(float maxSpeed, Vector2 currentPosition)
    {
        var proposed = _holdingPosition - currentPosition;
        
        if (proposed.Length() < 5)
            return Vector2.Zero;
        
        if (proposed.Length() > maxSpeed)
            return proposed.LimitLength(maxSpeed);
        
        if (proposed.Length() < maxSpeed / 2)
            return proposed.LimitLength(maxSpeed / 4);
        
        return proposed.LimitLength(maxSpeed / 2);
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