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
    
    public override string Name => $"{_destination.GetName(_player.GlobalPosition)} - {_state}";
    
    private readonly INavigable _destination;
    private readonly Player _player;

    private Marker2D _destinationApproach;
    
    private NavigationState _state = NavigationState.Start;

    public SystemAutoNavigation(Player ship, INavigable destination)
    {
        _player = ship;
        _destination = destination;
    }
    
    public override float GetRotation(float maxRotation) =>
        _player.Velocity.Angle();

    public override Vector2 GetVelocity(float maxSpeed)
    {
        if (Input.IsActionJustPressed("AutoPilotCancel"))
            SetState(NavigationState.Complete);
        
        return _state switch
        {
            NavigationState.Start => ProcessStarting(),
            NavigationState.Travelling => ProcessTravelling(maxSpeed),
            _ => ProcessComplete()
        };
    }
        
    // At the moment we basically don't need to deal with this.
    // If we implement warp or something we may need to revisit.
    public override void DisruptTravel() =>
        RaiseEvent(Aborted);

    private Vector2 ProcessStarting()
    {
        _destinationApproach = _destination.GetNearestMarker(_player.GlobalPosition);
        
        var proposed = _destinationApproach.GlobalPosition - _player.GlobalPosition;
        
        SetState(NavigationState.Travelling);

        return proposed.LimitLength(50);
    }

    private Vector2 ProcessTravelling(float maxSpeed)
    {
        var proposed = _destinationApproach.GlobalPosition - _player.GlobalPosition;
        
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
        Log.Debug("{Name} controlling {Ship} state change from {OldState} to {NewState}", Name, _player.Name, _state, newState);
        _state = newState;
    }
}