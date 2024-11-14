using Godot;
using Serilog;


namespace Spacelancer.Components.Navigation;

public class LaneNavigation : INavigationSoftware
{
    private enum LaneState
    {
        Initializing,
        Approaching,
        Entering,
        Travelling,
        Exiting,
        Complete,
    }
    
    public string Name => $"LaneNavigation - {_origin.Name} - {_state}";

    private readonly Scenes.Player.Player _player;
    private readonly Scenes.Spacelane.Spacelane _origin;
    private readonly Scenes.Spacelane.Spacelane _destination;

    private readonly INavigationSoftware _originalSoftware;
    
    private LaneState _state;
    
    // FIXME develop interface/class for space ships?
    // Could consider having some sort of sensor range to fire off events for nearby objects?
    public LaneNavigation(Scenes.Player.Player ship, Scenes.Spacelane.Spacelane origin, Scenes.Spacelane.Spacelane destination)
    {
        _player = ship;
        _origin = origin;
        _destination = destination;
        
        _originalSoftware = ship.NavComputer;
    }

    public float GetRotation(float maxRotation) =>
        _player.Velocity.Angle();

    public Vector2 GetVelocity(float maxSpeed) =>
        _state switch
        {
            LaneState.Initializing => ProcessInitializingVector(),
            LaneState.Approaching => ProcessApproachingVector(),
            LaneState.Entering => ProcessEnteringVector(),
            LaneState.Travelling => ProcessTravelingVector(),
            LaneState.Exiting => ProcessExitingVector(),
            LaneState.Complete => ProcessCompleteVector(),
            _ => Vector2.Zero
        };

    public void ExitLane()
    {
        SetState(LaneState.Complete);
    }

    private Vector2 ProcessInitializingVector()
    {
        var velocity = _player.Velocity;
        
        var x = Mathf.MoveToward(velocity.X, 0.0f, 5);
        var y = Mathf.MoveToward(velocity.Y, 0.0f, 5);
        
        var newVelocity = new Vector2(x, y);

        if (newVelocity.Length() < 5)
        {
            SetState(LaneState.Approaching);
            return Vector2.Zero;
        }
        
        return newVelocity;
    }

    private Vector2 ProcessApproachingVector()
    {
        var proposed = _origin.GlobalPosition - _player.GlobalPosition;

        if (proposed.Length() < 5)
        {
            SetState(LaneState.Entering);
            return Vector2.Zero;
        }
        
        return proposed.LimitLength(50);
    }
    
    private Vector2 ProcessEnteringVector()
    {
        var proposed = _destination.GlobalPosition - _player.GlobalPosition;
        var traveled = _origin.GlobalPosition - _player.GlobalPosition;

        if (traveled.Length() > 25)
        {
            SetState(LaneState.Travelling);
        }
        
        return proposed.LimitLength(10);
    }

    private Vector2 ProcessTravelingVector()
    {
        var proposed = _destination.GlobalPosition - _player.GlobalPosition;
        
        if (proposed.Length() < 5)
        {
            SetState(LaneState.Exiting);
            return proposed.LimitLength(10);
        }
        
        if (proposed.Length() < 100)
        {
            return proposed.LimitLength(50);
        }
        
        if (proposed.Length() < 250)
        {
            return proposed.LimitLength(150);
        }
        
        return proposed.LimitLength(750);
    }

    private Vector2 ProcessExitingVector()
    {
        // TODO probably need to consider things like nearby ships in future.
        // Use boiding/flocking?
        var proposed = _player.GlobalPosition - _origin.GlobalPosition;
        
        return proposed.LimitLength(50);
    }

    private Vector2 ProcessCompleteVector()
    {
        RestoreOriginalSoftware();
        return Vector2.Zero;
    }

    private void SetState(LaneState newState)
    {
        Log.Debug("{Name} controlling {Ship} state change from {OldState} to {NewState}", Name, _player.Name, _state, newState);
        _state = newState;
    }
    
    private void RestoreOriginalSoftware()
    {
        _player.NavComputer = _originalSoftware;
    }
}