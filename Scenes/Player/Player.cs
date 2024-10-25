using Godot;

public partial class Player : CharacterBody2D
{
	public const float MaxSpeed = 150.0f;

	public override void _PhysicsProcess(double delta)
	{
		ProcessRotation();

		if (Input.IsKeyPressed(Key.B))
			ProcessBrake();
		else
			ProcessVelocity();

		DebugVelocity();
		DebugSpeed(Velocity.Length());
		
		MoveAndSlide();
	}

	private void ProcessVelocity()
	{
		var velocity = Velocity;
		
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_down", "ui_up", "ui_left", "ui_right");

		// TODO clamp left and right (Y axis)
		// TODO clamp reverse (negative X)
		
		var acceleration = direction.Rotated(Rotation);
		DebugAcceleration(acceleration);

		if (direction != Vector2.Zero)
		{
			velocity = (Velocity + acceleration).LimitLength(MaxSpeed);
		}

		Velocity = velocity;
	}

	private void ProcessRotation()
	{
		// Rotate towards mouse
		var mouseCoords = GetGlobalMousePosition();
		var mouseDirection = mouseCoords - GlobalPosition;

		Rotation = mouseDirection.Angle();
		// TODO limit angle to +- x degrees
		
		DebugRotation();
	}

	private void ProcessBrake()
	{
		var x = Mathf.MoveToward(Velocity.X, 0.0f, 1);
		var y = Mathf.MoveToward(Velocity.Y, 0.0f, 1);
		
		var velocity = new Vector2(x, y);
		
		var brakeForce = velocity - Velocity;
		DebugAcceleration(brakeForce);

		Velocity = velocity;
	}

	// TODO debug options
	// Navigation system
	// health
	
	// TODO Brakes
	// Press B or something to move Velocity towards 0
	
	// Probably need to extract some of these things for enemies and the like too.
}
