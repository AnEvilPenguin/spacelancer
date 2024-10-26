using System;
using Godot;
using Serilog;

namespace Spacelancer.Components.Navigation;

public class PlayerNavigation : INavigationSoftware
{
    
    public string Name { get => _name; }
    
    private const string _name = "PlayerNavigation";
    private readonly Player _player;

    public PlayerNavigation(Player player)
    {
        _player = player;
    }
    
    public float GetRotation(float maxRotation)
    {
        ValidatePlayer();
        
        // TODO work out how max rotation should work
        var mouseCoords = _player.GetGlobalMousePosition();
        var mouseDirection = mouseCoords - _player.GlobalPosition;

        return mouseDirection.Angle();
    }

    public Vector2 GetVelocity(float maxSpeed)
    {
        ValidatePlayer();
        
        var velocity = Input.IsKeyPressed(Key.B) ? ProcessBrake() : ProcessControls();
        return velocity.LimitLength(maxSpeed);
    }

    private void ValidatePlayer()
    {
        if (_player == null)
            throw new NotSupportedException("Only Player is supported");
    }
    
    private Vector2 ProcessControls()
    {
        var velocity = _player.Velocity;
		
        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("ui_down", "ui_up", "ui_left", "ui_right");

        // TODO clamp left and right (Y axis)
        // TODO clamp reverse (negative X)
		
        var acceleration = direction.Rotated(_player.Rotation);

        if (direction != Vector2.Zero)
        {
            velocity = (velocity + acceleration);
        }
        
        return velocity;
    }

    private Vector2 ProcessBrake()
    {
        var velocity = _player.Velocity;
        
        var x = Mathf.MoveToward(velocity.X, 0.0f, 1);
        var y = Mathf.MoveToward(velocity.Y, 0.0f, 1);
		
        return new Vector2(x, y);
    }
}