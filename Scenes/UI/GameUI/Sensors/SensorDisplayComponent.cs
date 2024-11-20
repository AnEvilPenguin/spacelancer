using Godot;
using Serilog;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Controllers;

namespace Spacelancer.Scenes.UI.GameUI.Sensors;

public partial class SensorDisplayComponent : PanelContainer
{
	[Signal]
	public delegate void SelectedEventHandler(Node2D selectedNode);
	
	private Label _nameLabel;
	private Label _distanceLabel;
	
	private SensorDetection _detection;
	
	public SensorDetectionType DetectionType { get; private set; }
	public float Distance { get; private set; }
	
	public override void _Ready()
	{
		_nameLabel = GetNode<Label>("%NameLabel");
		_distanceLabel = GetNode<Label>("%DistanceLabel");
	}

	public override void _Input(InputEvent @event)
	{
		if (!Visible || @event is not InputEventMouseButton eventMouseButton || eventMouseButton.ButtonIndex != MouseButton.Left)
			return;
		
		if (!IsWithinBounds(GetLocalMousePosition()))
			return;
		
		Log.Debug("Player selected {detectionId} - {detectionType}", _detection.Id, _detection.ReturnType);
		
		EmitSignal(SignalName.Selected, _detection.Body);
		
		// Stops propagation of event
		GetViewport().SetInputAsHandled();
	}

	public void SetSensorDetection(SensorDetection detection)
	{
		_detection = detection;
		// TODO some sort of icon based on return type?
		DetectionType = detection.ReturnType;

		UpdateComponent();
		
		if (DetectionType is SensorDetectionType.Undetectable or SensorDetectionType.SpaceLaneNode)
			Visible = false;
	}

	public SensorDisplayComponent UpdateComponent()
	{
		_nameLabel.Text = _detection.Name;

		Distance = _detection.Body.GlobalPosition.DistanceTo(Global.Player.GlobalPosition);
		_distanceLabel.Text = Distance >= 2000 ? $"{Distance / 1000 :0.0}K" : $"{Distance:0}";
		
		return this;
	}

	private bool IsWithinBounds(Vector2 position)
	{
		if (position.X < 0 || position.Y < 0)
			return false;
		
		if (position.X > Size.X || position.Y > Size.Y)
			return false;
		
		return true;
	}
}