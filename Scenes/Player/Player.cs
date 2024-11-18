using Godot;

namespace Spacelancer.Scenes.Player;

public partial class Player : CharacterBody2D
{
	public const float MaxSpeed = 150.0f;

	public override void _Ready()
	{
		SetDefaultEquipment();
	}

	public override void _PhysicsProcess(double delta)
	{
		DebugNav();
		
		// TODO work out what a sensible max rotation is
		Rotation = NavComputer.GetRotation(0);
		DebugRotation();
		
		ProcessVelocity();
		
		
		MoveAndSlide();
	}

	private void ProcessVelocity()
	{
		var velocity = NavComputer.GetVelocity(MaxSpeed);

		var acceleration = velocity - Velocity;
		DebugAcceleration(acceleration);
		
		Velocity = velocity;
		
		DebugVelocity();
		DebugSpeed(Velocity.Length());
	}
}