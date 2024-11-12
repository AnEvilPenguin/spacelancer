using Godot;
using Serilog;

namespace Spacelancer.Components.Navigation;

public class JumpNavigation : INavigationSoftware
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
    public string Name => $"JumpNavigation - {_state}";
    
    private readonly Scenes.Player.Player _player;
    private readonly Scenes.JumpGate.JumpGate _origin;
    private Scenes.JumpGate.JumpGate _exit;
    private readonly string _destination;
    
    private Node2D _destinationNode;
    private Vector2 _exitMarker;
    
    private readonly INavigationSoftware _originalSoftware;

    private JumpState _state;
    
    public JumpNavigation(Scenes.Player.Player ship, Scenes.JumpGate.JumpGate origin, string destination)
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
            JumpState.Initializing => ProcessInitializingVector(),
            JumpState.Approaching => ProcessApproachingVector(),
            JumpState.Entering => ProcessEnteringVector(),
            JumpState.Travelling => ProcessTravellingVector(),
            JumpState.Exiting => ProcessExitingVector(),
            JumpState.Complete => ProcessCompleteVector(),
            _ => Vector2.Zero
        };
    
    private Vector2 ProcessInitializingVector()
    {
        var velocity = _player.Velocity;
        
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

    private Vector2 ProcessApproachingVector()
    {
        var proposed = _origin.GlobalPosition - _player.GlobalPosition;
        
        if (proposed.Length() < 5)
        {
            SetState(JumpState.Entering);
            return Vector2.Zero;
        }
        
        return proposed.LimitLength(50);
    }

    private Vector2 ProcessEnteringVector()
    {
        if (_destinationNode == null)
        {
            _destinationNode = Controllers.Global.GameController.LoadScene<Node2D>($"res://Scenes/Systems/{_origin.Name.ToString().ToLower()}.tscn");
            _destinationNode.Visible = false;
        }

        if ((_player.GlobalPosition - _origin.GlobalPosition).Length() < 25)
        {
            SetState(JumpState.Travelling);
            return Vector2.Zero;
        }
        
        return _player.Velocity.LimitLength(5);
    }

    private Vector2 ProcessTravellingVector()
    {
        var oldSystem = _origin.GetParent<Node2D>();
        
        _exit = _destinationNode.GetNode<Scenes.JumpGate.JumpGate>($"{oldSystem.Name}");
        _exitMarker = _exit.GetExitMarker();
        
        _destinationNode.Visible = true;
        oldSystem.Visible = false;
        
        _player.GlobalPosition = _exit.GlobalPosition;
        
        Controllers.Global.GameController.UnloadScene(oldSystem);
        
        SetState(JumpState.Exiting);
        
        return Vector2.Zero;
    }

    private Vector2 ProcessExitingVector()
    {
        var proposed = _exitMarker - _player.GlobalPosition;
        
        if (proposed.Length() < 5)
        {
            SetState(JumpState.Complete);
            return Vector2.Zero;
        }
        
        return proposed.LimitLength(50);
    }
    
    private Vector2 ProcessCompleteVector()
    {
        RestoreOriginalSoftware();
        return Vector2.Zero;
    }
    
    private void SetState(JumpState newState)
    {
        Log.Debug("{Name} controlling {Ship} state change from {OldState} to {NewState}", Name, _player.Name, _state, newState);
        _state = newState;
    }
    
    private void RestoreOriginalSoftware() =>
        _player.NavComputer = _originalSoftware;
}