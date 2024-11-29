using Godot;

namespace Spacelancer.Scenes.SpaceShips;

public partial class Player : Ship
{
	private SensorPointer _pointer;

	public override void _Ready()
	{
		_pointer = GetNode<SensorPointer>("SensorPointer");
		SetDefaultEquipment();
	}

	public override void _Process(double delta) => 
		_navComputer.CheckForNavigationInstructions();
	
	private void SetPointerTarget(Node2D target) =>
		_pointer.SetTarget(target);
	
	private void ClearPointerTarget() =>
		_pointer.ClearTarget();
}