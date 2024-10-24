using Godot;

public partial class Player : CharacterBody2D
{
	public const float MaxSpeed = 150.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Rotate towards mouse
		var mouseCoords = GetGlobalMousePosition();
		var mouseDirection = mouseCoords - GlobalPosition;

		Rotation = mouseDirection.Angle();
		// TODO limit angle to +- x degrees
		
		DebugRotation();

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_down", "ui_up", "ui_left", "ui_right");

		// TODO clamp left and right (Y axis)
		// TODO clamp reverse (negative X)


		GD.Print($"direction: {direction}");
		GD.Print($"rotation: {Mathf.RadToDeg(Rotation)}");
		
		var acceleration = direction.Rotated(Rotation);
		DebugAcceleration(acceleration);

		if (direction != Vector2.Zero)
		{
			velocity = (Velocity + acceleration).LimitLength(MaxSpeed);
		}
		
		DebugSpeed(velocity.Length());

		GD.Print("Velocity: " + velocity);

		Velocity = velocity;
		DebugVelocity();
		
		MoveAndSlide();
	}

	// TODO debug options
	// Navigation system
	// health
	
	// TODO Brakes
	// Press B or something to move Velocity towards 0
	
	// Probably need to extract some of these things for enemies and the like too.
}
