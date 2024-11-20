using System.Collections.Generic;
using System.Linq;
using Godot;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Controllers;
using Spacelancer.Scenes.UI.StationMenu.IconButton;

namespace Spacelancer.Scenes.UI.GameUI.Sensors;

public partial class SensorDisplay : PanelContainer
{
	private enum DisplayType
	{
		Important,
		Stations,
		Navigation,
		All
	}
	
	private readonly Dictionary<ulong, SensorDisplayComponent> _objectControlLookup = new();
	private readonly List<SensorDisplayComponent> _objectControls = new();

	private readonly PackedScene _displayComponentScene =
		GD.Load<PackedScene>("res://Scenes/UI/GameUI/Sensors/sensor_display_component.tscn");
	
	private Timer _updateTimer;
	
	private IconButton _important;
	private IconButton _stations;
	private IconButton _navigation;
	private IconButton _all;

	private VBoxContainer _objectList;
	
	private DisplayType _displayType = DisplayType.Important;

	private SensorDisplayComponent _selectedComponent;

	public override void _Ready()
	{
		_updateTimer = GetNode<Timer>("%UpdateTimer");
		_updateTimer.Timeout += OnUpdateTimerTimeout;
		
		_important = GetNode<IconButton>("%Important");
		_important.Pressed += () => { _displayType = DisplayType.Important; FilterAllControls(); };
			
		_stations = GetNode<IconButton>("%Stations");
		_stations.Pressed += () => { _displayType = DisplayType.Stations; FilterAllControls(); };
		
		_navigation = GetNode<IconButton>("%Navigation");
		_navigation.Pressed += () => { _displayType = DisplayType.Navigation; FilterAllControls(); };
		
		_all = GetNode<IconButton>("%All");
		_all.Pressed += () => { _displayType = DisplayType.All; FilterAllControls(); };
		
		_objectList = GetNode<VBoxContainer>("%ObjectList");
	}

	private void FilterAllControls() =>
		_objectControls.ForEach(FilterControl);

	private void FilterControl(SensorDisplayComponent control)
	{
		switch (_displayType)
		{
			case DisplayType.Important:
				control.Visible = control.DetectionType is not (SensorDetectionType.Undetectable or SensorDetectionType.SpaceLaneNode);
				return;
			
			case DisplayType.Stations:
				control.Visible = control.DetectionType is SensorDetectionType.Station;
				return;
			
			case DisplayType.Navigation:
				control.Visible = control.DetectionType is SensorDetectionType.JumpGate or SensorDetectionType.SpaceLane;
				return;
			
			default:
				control.Visible = true;
				return;
		}
	}

	private void OnUpdateTimerTimeout()
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
		
		_objectControls.Select(c => c.UpdateComponent())
			.OrderBy(c => c.Distance)
			.ToList()
			.ForEach(c => _objectList.AddChild(c));
	}

	public void AddItem(SensorDetection detection)
	{
		var body = detection.Body;
		var id = body.GetInstanceId();

		if (_objectControlLookup.ContainsKey(id))
			return;

		var component = _displayComponentScene.Instantiate<SensorDisplayComponent>();
		_objectList.AddChild(component);
		
		// component must enter scene before detection is set
		component.Name = id.ToString();
		component.SetSensorDetection(detection);

		component.Selected += (Node2D selected) =>
		{
			_selectedComponent = component;
			Global.Player.SetPointerTarget(selected);
		};
		
		FilterControl(component);
		
		_objectControlLookup.Add(id, component);
		_objectControls.Add(component);
	}

	public void RemoveItem(ulong id)
	{
		// consider logging error?
		if (!_objectControlLookup.Remove(id, out var control))
			return;
		
		if (_selectedComponent == control)
			Global.Player.ClearPointerTarget();
		
		control.Visible = false;
		control.QueueFree();
		
		_objectControls.Remove(control);
	}
}