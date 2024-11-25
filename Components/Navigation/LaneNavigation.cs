using System;
using Godot;
using Serilog;

namespace Spacelancer.Components.Navigation;

public class LaneNavigation : AutomatedNavigation
{
    public override event EventHandler Complete;
    public override event EventHandler Aborted;
    
    private enum LaneState
    {
        Initializing,
        Approaching,
        Entering,
        Travelling,
        Exiting,
        Disrupted,
        Complete
    }
    
    public override string Name => $"LaneNavigation - {_origin.Name} - {_state}";
    public override NavigationSoftwareType Type => NavigationSoftwareType.Docking;
    
    private readonly Node2D _origin;
    private readonly Node2D _destination;
    
    private LaneState _state;
    
    // Could consider having some sort of sensor range to fire off events for nearby objects?
    public LaneNavigation(Node2D origin, Node2D destination)
    {
        _origin = origin;
        _destination = destination;
    }

    public override float GetRotation(float maxRotation, float currentRotation, Vector2 currentVelocity) =>
        currentVelocity.Angle();

    public override Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity)
    {
        if (Input.IsActionJustPressed("AutoPilotCancel"))
            DisruptTravel();
        
        return _state switch
        {
            LaneState.Initializing => ProcessInitializingVector(currentVelocity),
            LaneState.Approaching => ProcessApproachingVector(currentPosition),
            LaneState.Entering => ProcessEnteringVector(currentPosition),
            LaneState.Travelling => ProcessTravelingVector(currentPosition),
            LaneState.Exiting => ProcessExitingVector(currentPosition),
            LaneState.Disrupted => ProcessDisruptedVector(currentVelocity),
            LaneState.Complete => ProcessCompleteVector(currentVelocity),
            _ => Vector2.Zero
        };
    }

    public override void DisruptTravel()
    {
        SetState(LaneState.Disrupted);
        Log.Debug("{Name} - Nav disrupted", Name);
    }

    private Vector2 ProcessInitializingVector(Vector2 currentVelocity)
    {
        var velocity = currentVelocity;
        
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

    private Vector2 ProcessApproachingVector(Vector2 currentPosition)
    {
        var proposed = _origin.GlobalPosition - currentPosition;

        if (proposed.Length() < 5)
        {
            SetState(LaneState.Entering);
            return Vector2.Zero;
        }
        
        return proposed.LimitLength(50);
    }
    
    private Vector2 ProcessEnteringVector(Vector2 currentPosition)
    {
        var proposed = _destination.GlobalPosition - currentPosition;
        var traveled = _origin.GlobalPosition - currentPosition;

        if (traveled.Length() > 25)
        {
            SetState(LaneState.Travelling);
        }
        
        return proposed.LimitLength(10);
    }

    private Vector2 ProcessTravelingVector(Vector2 currentPosition)
    {
        var proposed = _destination.GlobalPosition - currentPosition;
        
        if (proposed.Length() < 5)
        {
            SetState(LaneState.Exiting);
        }
        
        if (proposed.Length() < 100)
        {
            return proposed.LimitLength(50);
        }
        
        if (proposed.Length() < 250)
        {
            return proposed.LimitLength(150);
        }
        
        if (proposed.Length() < 1000)
        {
            return proposed.LimitLength(750);
        }
        
        return proposed.LimitLength(2500);
    }

    private Vector2 ProcessExitingVector(Vector2 currentPosition)
    {
        if ((_destination.GlobalPosition - currentPosition).Length() > 150)
            SetState(LaneState.Complete);
        
        // TODO probably need to consider things like nearby ships in future.
        // Use boiding/flocking?
        var proposed = currentPosition - _origin.GlobalPosition;
        
        return proposed.LimitLength(50);
    }

    private Vector2 ProcessDisruptedVector(Vector2 currentVelocity)
    {
        var proposed = currentVelocity;
        
        // slowly veer off to left
        proposed += proposed.Orthogonal() / 50;
        
        var length = proposed.Length();

        if (length <= 25)
            RaiseEvent(Aborted);
        
        // slow down 
        return proposed.LimitLength(length * 0.95f);
    }

    private Vector2 ProcessCompleteVector(Vector2 currentVelocity)
    {
        RaiseEvent(Complete);

        return currentVelocity;
    }

    private void SetState(LaneState newState)
    {
        Log.Debug("{Name} state change from {OldState} to {NewState}", Name, _state, newState);
        _state = newState;
    }
}