using System;
using Godot;
using Serilog;
using Spacelancer.Scenes.Transitions;

namespace Spacelancer.Components.Navigation.Software;

public class JumpEntranceNavigation : AutomatedNavigation
{
    private enum JumpState
    {
        Initializing,
        Approaching,
        Entering,
        Complete,
    }

    public override event EventHandler Complete;
    public override event EventHandler Aborted;
    public event EventHandler<JumpEventArgs> Jumping; 

    public override string Name => $"JumpEntrance - {_state}";
    public override NavigationSoftwareType Type => NavigationSoftwareType.Docking;
    
    private readonly JumpGate _origin;
    private readonly string _originalSystem;
    private readonly string _destination;

    private JumpState _state;
    
    public JumpEntranceNavigation(JumpGate origin, string destination)
    {
        _origin = origin;
        _destination = destination;

        _originalSystem = origin.GetParent().Name;
    }
    
    public override float GetRotation(float maxRotation, float currentRotation, Vector2 currentVelocity) =>
        currentVelocity.Angle();

    public override Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity) =>
        _state switch
        {
            JumpState.Initializing => ProcessInitializingVector(currentVelocity),
            JumpState.Approaching => ProcessApproachingVector(currentPosition),
            JumpState.Entering => ProcessEnteringVector(currentPosition, currentVelocity),
            JumpState.Complete => ProcessCompleteVector(),
            _ => Vector2.Zero
        };
    
    public override void DisruptTravel() =>
        RaiseEvent(Aborted);
    
    private Vector2 ProcessInitializingVector(Vector2 currentVelocity)
    {
        var velocity = currentVelocity;
        
        var x = Mathf.MoveToward(velocity.X, 0.0f, 5);
        var y = Mathf.MoveToward(velocity.Y, 0.0f, 5);
        
        var newVelocity = new Vector2(x, y);

        if (newVelocity.Length() < 5)
        {
            SetState(JumpState.Approaching);
            return Vector2.Zero;
        }
        
        return newVelocity;
    }

    private Vector2 ProcessApproachingVector(Vector2 currentPosition)
    {
        var proposed = _origin.GlobalPosition - currentPosition;
        
        if (proposed.Length() < 5)
        {
            SetState(JumpState.Entering);
            return Vector2.Zero;
        }
        
        return proposed.LimitLength(50);
    }

    private Vector2 ProcessEnteringVector(Vector2 currentPosition, Vector2 currentVelocity)
    {
        if ((currentPosition - _origin.GlobalPosition).Length() < 25)
        {
            SetState(JumpState.Complete);
            return Vector2.Zero;
        }
        
        return currentVelocity.LimitLength(5);
    }
    
    private Vector2 ProcessCompleteVector()
    {
        var raiseEvent = Jumping;
        raiseEvent?.Invoke(this, new JumpEventArgs(_destination, _originalSystem));
        
        return Vector2.Zero;
    }
    
    private void SetState(JumpState newState)
    {
        Log.Debug("{Name} state change from {OldState} to {NewState}", Name, _state, newState);
        _state = newState;
    }
}