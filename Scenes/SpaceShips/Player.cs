using Godot;
using Spacelancer.Components.Equipment.Detection;

namespace Spacelancer.Scenes.SpaceShips;

public partial class Player : Ship, ISensorDetectable
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
	
	public SensorDetectionType ReturnType =>
		SensorDetectionType.Ship;

	public string Affiliation =>
		"TODO";

	public string GetName(Vector2 _) =>
		Name;

	public Node2D ToNode2D() =>
		this as Node2D;
}