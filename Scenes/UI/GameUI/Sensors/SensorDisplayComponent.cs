using Godot;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Controllers;

namespace Spacelancer.Scenes.UI.GameUI.Sensors;

public partial class SensorDisplayComponent : PanelContainer
{
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
}