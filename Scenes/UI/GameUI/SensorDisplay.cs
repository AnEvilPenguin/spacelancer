using System.Collections.Generic;
using Godot;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Controllers;
using Spacelancer.Scenes.UI.StationMenu.IconButton;

namespace Spacelancer.Scenes.UI.GameUI;

public partial class SensorDisplay : PanelContainer
{
	private readonly Dictionary<ulong, Control> _objectUiControls = new();
	private readonly List<SensorDetection> _objectDetections = new();
	
	private IconButton _important;
	private IconButton _stations;
	private IconButton _navigation;
	private IconButton _all;

	private VBoxContainer _objectList;

	public override void _Ready()
	{
		_important = GetNode<IconButton>("%Important");
		_stations = GetNode<IconButton>("%Stations");
		_navigation = GetNode<IconButton>("%Navigation");
		_all = GetNode<IconButton>("%All");
		
		_objectList = GetNode<VBoxContainer>("%ObjectList");
	}

	public void AddItem(SensorDetection detection)
	{
		var body = detection.Body;
		var id = body.GetInstanceId();

		if (_objectUiControls.ContainsKey(id))
			return;
		
		_objectDetections.Add(detection);
		
		// TODO proper component here
		var component = new HBoxContainer();
		
		var nameLabel = new Label();
		nameLabel.Text = detection.Name;
		
		// TODO need to update this regularly
		var distanceLabel = new Label();
		var distance = body.GlobalPosition.DistanceTo(Global.Player.GlobalPosition) / 1000;
		distanceLabel.Text = $"{distance:0.0}";
		
		component.AddChild(nameLabel);
		component.AddChild(distanceLabel);
		
		_objectUiControls.Add(id, component);
		_objectList.AddChild(component);
	}

	public void RemoveItem(ulong id)
	{
		// consider logging error?
		if (!_objectUiControls.Remove(id, out var control))
			return;

		control.Visible = false;
		control.QueueFree();
	}

	// TODO remove component
	// TODO change visibility based on type
	// TODO update labels
	// TODO sort by distance
}