using System;
using Godot;
using Serilog;
using Spacelancer.Scenes.Player;

namespace Spacelancer.Components.Navigation;

public class SystemAutoNavigation : AutomatedNavigation
{
    public override event EventHandler Complete;
    public override event EventHandler Aborted;
    
    private enum NavigationState
    {
        Start,
        Travelling,
        Complete
    }
    
    public override string Name => _name;

    private string _name = string.Empty;

    public override NavigationSoftwareType Type => NavigationSoftwareType.Navigation;

    private readonly INavigable _destination;

    private Marker2D _destinationApproach;
    
    private NavigationState _state = NavigationState.Start;

    public SystemAutoNavigation(INavigable destination)
    {
        _destination = destination;
    }
    
    public override float GetRotation(float maxRotation, float currentAngleRads, Vector2 currentVelocity) =>
        currentVelocity.Angle();

    public override Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity)
    {
        _name = $"{_destination.GetName(currentPosition)} - {_state}";
        
        if (Input.IsActionJustPressed("AutoPilotCancel"))
            SetState(NavigationState.Complete);
        
        return _state switch
        {
            NavigationState.Start => ProcessStarting(currentPosition),
            NavigationState.Travelling => ProcessTravelling(maxSpeed, currentPosition),
            _ => ProcessComplete()
        };
    }
        
    // At the moment we basically don't need to deal with this.
    // If we implement warp or something we may need to revisit.
    public override void DisruptTravel() =>
        RaiseEvent(Aborted);

    private Vector2 ProcessStarting(Vector2 currentPosition)
    {
        _destinationApproach = _destination.GetNearestMarker(currentPosition);
        
        var proposed = _destinationApproach.GlobalPosition - currentPosition;
        
        SetState(NavigationState.Travelling);

        return proposed.LimitLength(50);
    }

    private Vector2 ProcessTravelling(float maxSpeed, Vector2 currentPosition)
    {
        var proposed = _destinationApproach.GlobalPosition - currentPosition;
        
        if (proposed.Length() < 10)
        {
            SetState(NavigationState.Complete);
            return proposed.LimitLength(10);
        }
        
        if (proposed.Length() < 50)
        {
            return proposed.LimitLength(50);
        }
        
        return proposed.Length() < maxSpeed * 2 ? 
            proposed.LimitLength(maxSpeed / 2) : 
            proposed.LimitLength(maxSpeed);
    }

    private Vector2 ProcessComplete()
    {
        RaiseEvent(Complete);
        return Vector2.Zero;
    }

    private void SetState(NavigationState newState)
    {
        Log.Debug("{Name} state change from {OldState} to {NewState}", Name, _state, newState);
        _state = newState;
    }
}