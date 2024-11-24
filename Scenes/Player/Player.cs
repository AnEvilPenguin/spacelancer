using Godot;

namespace Spacelancer.Scenes.Player;

public partial class Player : CharacterBody2D
{
	public const float MaxSpeed = 150.0f;
	private SensorPointer _pointer;

	public override void _Ready()
	{
		_pointer = GetNode<SensorPointer>("SensorPointer");
		SetDefaultEquipment();
	}

	public override void _PhysicsProcess(double delta)
	{
		DebugNav();
		
		// TODO work out what a sensible max rotation is
		Rotation = NavComputer.GetRotation(0, Rotation, Velocity);
		DebugRotation();
		
		ProcessVelocity();
		
		
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		NavComputer.CheckForNavigationInstructions();
	}

	private void ProcessVelocity()
	{
		var velocity = NavComputer.GetVelocity(MaxSpeed, GlobalPosition, Velocity);

		var acceleration = velocity - Velocity;
		DebugAcceleration(acceleration);
		
		Velocity = velocity;
		
		DebugVelocity();
		DebugSpeed(Velocity.Length());
	}
	
	private void SetPointerTarget(Node2D target) =>
		_pointer.SetTarget(target);
	
	private void ClearPointerTarget() =>
		_pointer.ClearTarget();
}