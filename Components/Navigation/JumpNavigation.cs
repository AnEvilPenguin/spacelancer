using System;
using Godot;
using Serilog;
using Spacelancer.Controllers;
using Spacelancer.Scenes.Transitions;
using Spacelancer.Scenes.Player;

namespace Spacelancer.Components.Navigation;

public class JumpNavigation : AutomatedNavigation
{
    private enum JumpState
    {
        Initializing,
        Approaching,
        Entering,
        Travelling,
        Exiting,
        Complete
    }

    public override event EventHandler Complete;
    public override event EventHandler Aborted;

    public override string Name => $"JumpNavigation - {_state}";
    public override NavigationSoftwareType Type => NavigationSoftwareType.Docking;
    
    private readonly JumpGate _origin;
    private readonly string _originalSystem;
    private JumpGate _exit;
    private readonly string _destination;
    
    private Node2D _destinationNode;
    private Vector2 _exitMarker;
    

    private JumpState _state;
    
    public JumpNavigation(JumpGate origin, string destination)
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
            JumpState.Travelling => ProcessTravellingVector(),
            JumpState.Exiting => ProcessExitingVector(currentPosition),
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
        if (_destinationNode == null)
        {
            var destinationId = Global.Universe.GetSystemId(_destination);
            _destinationNode = Global.GameController.LoadSystem(destinationId);
        }

        if ((currentPosition - _origin.GlobalPosition).Length() < 25)
        {
            SetState(JumpState.Travelling);
            return Vector2.Zero;
        }
        
        return currentVelocity.LimitLength(5);
    }

    private Vector2 ProcessTravellingVector()
    {
        _exit = _destinationNode.GetNode<JumpGate>($"{_originalSystem}");
        _exitMarker = _exit.GetExitMarker();
        
        _destinationNode.Visible = true;
        
        // FIXME we probably need a method of setting the target ship for cases like this
        // new signal up for 'warp/teleport' or something
        Global.Player.GlobalPosition = _exit.GlobalPosition;
        
        SetState(JumpState.Exiting);
        
        return Vector2.Zero;
    }

    private Vector2 ProcessExitingVector(Vector2 currentPosition)
    {
        var proposed = _exitMarker - currentPosition;
        
        if (proposed.Length() < 5)
        {
            SetState(JumpState.Complete);
            return Vector2.Zero;
        }
        
        return proposed.LimitLength(50);
    }
    
    private Vector2 ProcessCompleteVector()
    {
        RaiseEvent(Complete);
        return Vector2.Zero;
    }
    
    private void SetState(JumpState newState)
    {
        Log.Debug("{Name} state change from {OldState} to {NewState}", Name, _state, newState);
        _state = newState;
    }
}