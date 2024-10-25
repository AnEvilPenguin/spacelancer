using Godot;
using Spacelancer.Components.Navigation;

public partial class Player : CharacterBody2D
{
	public const float MaxSpeed = 150.0f;

	private INavigationSoftware _navComputer;

	public override void _Ready()
	{
		_navComputer = new PlayerNavigation(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		// TODO work out what a sensible max rotation is
		Rotation = _navComputer.GetRotation(0);
		DebugRotation();
		
		ProcessVelocity();
		
		MoveAndSlide();
	}

	private void ProcessVelocity()
	{
		var velocity = _navComputer.GetVelocity(MaxSpeed);

		var acceleration = velocity - Velocity;
		DebugAcceleration(acceleration);
		
		Velocity = velocity;
		
		DebugVelocity();
		DebugSpeed(Velocity.Length());
	}
	
	// TODO debug options
	// Navigation system
	// health
	
	// TODO Brakes
	// Press B or something to move Velocity towards 0
	
	// Probably need to extract some of these things for enemies and the like too.
}
