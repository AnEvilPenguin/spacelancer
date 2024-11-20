using Godot;
using Serilog;
using Spacelancer.Scenes.Player;

namespace Spacelancer.Components.Navigation;

public class SystemAutoNavigation : INavigationSoftware
{
    private enum NavigationState
    {
        Start,
        Travelling,
        Complete
    }
    
    public string Name => $"{_destination.GetName(_player.GlobalPosition)} - {_state}";
    
    private readonly INavigable _destination;
    private readonly Player _player;
    private readonly INavigationSoftware _nextNavigation;

    private Marker2D _destinationApproach;
    
    private NavigationState _state = NavigationState.Start;

    public SystemAutoNavigation(Player ship, INavigable destination)
    {
        _player = ship;
        _destination = destination;
        _nextNavigation = _player.NavComputer;
    }

    public SystemAutoNavigation(Player ship, INavigable destination, INavigationSoftware nextNavigation)
    {
        _player = ship;
        _destination = destination;
        _nextNavigation = nextNavigation;
    }
    
    public float GetRotation(float maxRotation) =>
        _player.Velocity.Angle();

    public Vector2 GetVelocity(float maxSpeed)
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
        _player.NavComputer = _nextNavigation;
        return Vector2.Zero;
    }

    private void SetState(NavigationState newState)
    {
        Log.Debug("{Name} controlling {Ship} state change from {OldState} to {NewState}", Name, _player.Name, _state, newState);
        _state = newState;
    }
}