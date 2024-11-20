using System.Collections.Generic;
using System.Linq;
using Godot;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Controllers;
using Spacelancer.Scenes.UI.StationMenu.IconButton;

namespace Spacelancer.Scenes.UI.GameUI.Sensors;

public partial class SensorDisplay : PanelContainer
{
	private readonly Dictionary<ulong, Control> _objectControlLookup = new();
	private readonly List<Control> _objectControls = new();
	private readonly List<SensorDetection> _objectDetections = new();
	
	private IconButton _important;
	private IconButton _stations;
	private IconButton _navigation;
	private IconButton _all;

	private VBoxContainer _objectList;

	public override void _Ready()
	{
		_important = GetNode<IconButton>("%Important");
		_important.Pressed += () => _objectDetections.ForEach(d =>
			{
				// FIXME just make id a property of detection for ease of use
				var id = d.Body.GetInstanceId();
				var control = _objectControlLookup[id];

				if (d.ReturnType is SensorDetectionType.Undetectable or SensorDetectionType.SpaceLaneNode)
					control.Visible = false;
				else
					control.Visible = true;
			});
			
		_stations = GetNode<IconButton>("%Stations");
		_stations.Pressed += () => _objectDetections.ForEach(d =>
			{
				// FIXME just make id a property of detection for ease of use
				var id = d.Body.GetInstanceId();
				var control = _objectControlLookup[id];

				if (d.ReturnType is SensorDetectionType.Station)
					control.Visible = true;
				else
					control.Visible = false;
			});
		
		_navigation = GetNode<IconButton>("%Navigation");
		_navigation.Pressed += () => _objectDetections.ForEach(d =>
			{
				// FIXME just make id a property of detection for ease of use
				var id = d.Body.GetInstanceId();
				var control = _objectControlLookup[id];

				if (d.ReturnType is SensorDetectionType.JumpGate or SensorDetectionType.SpaceLane)
					control.Visible = true;
				else
					control.Visible = false;
			});
		
		_all = GetNode<IconButton>("%All");
		_all.Pressed += () => _objectControls.ForEach(c => c.Visible = true);
		
		_objectList = GetNode<VBoxContainer>("%ObjectList");
	}

	public override void _Process(double delta)
	{
		if (Global.IsClosing)
			return;
		
		UpdateAllComponents();
	}

	private void UpdateAllComponents()
	{
		if (_objectControls.Count == 0)
			return;
		
		_objectControls.ForEach(_objectList.RemoveChild);
		
		_objectDetections.Where(d => IsInstanceValid(d.Body))
			.OrderBy(d => d.Body.GlobalPosition.DistanceTo(Global.Player.GlobalPosition))
			.ToList()
			.ForEach(d =>
			{
				var body = d.Body;
				
				var distance = body.GlobalPosition.DistanceTo(Global.Player.GlobalPosition);
				
				var component = _objectControlLookup[body.GetInstanceId()];
				
				UpdateComponent(component, distance);
				_objectList.AddChild(component);
			});
		
	}

	private void UpdateComponent(Control control, float distance)
	{
		var distanceLabel = control.GetNode<Label>("distanceLabel");
		
		distanceLabel.Text = distance >= 2000 ? $"{distance / 1000 :0.0}K" : $"{distance:0}";
	}

	public void AddItem(SensorDetection detection)
	{
		var body = detection.Body;
		var id = body.GetInstanceId();

		if (_objectControlLookup.ContainsKey(id))
			return;
		
		_objectDetections.Add(detection);
		
		// TODO proper component here
		var component = new HBoxContainer();
		
		var nameLabel = new Label();
		nameLabel.Name = "nameLabel";
		nameLabel.Text = detection.Name;
		
		// TODO need to update this regularly
		// Can we get the component to update itself?
		var distanceLabel = new Label();
		distanceLabel.Name = "distanceLabel";
		var distance = body.GlobalPosition.DistanceTo(Global.Player.GlobalPosition) / 1000;
		distanceLabel.Text = $"{distance:0}";
		
		component.AddChild(nameLabel);
		component.AddChild(distanceLabel);

		if (detection.ReturnType is SensorDetectionType.Undetectable or SensorDetectionType.SpaceLaneNode)
			component.Visible = false;
		
		_objectControlLookup.Add(id, component);
		_objectControls.Add(component);
		_objectList.AddChild(component);
	}

	public void RemoveItem(ulong id)
	{
		// consider logging error?
		if (!_objectControlLookup.Remove(id, out var control))
			return;
		
		control.Visible = false;
		control.QueueFree();
		
		_objectControls.Remove(control);
		
		var detection = _objectDetections
			.First(d => d.Body.GetInstanceId() == id);
		_objectDetections.Remove(detection);
	}

	// TODO remove component
	// TODO change visibility based on type
	// TODO update labels
	// TODO sort by distance
}