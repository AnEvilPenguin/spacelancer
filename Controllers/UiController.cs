using Godot;
using Serilog;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Navigation;
using Spacelancer.Scenes.UI.GameUI.Navigation;
using Spacelancer.Scenes.UI.GameUI.Sensors;
using SensorDisplay = Spacelancer.Scenes.UI.GameUI.Sensors.SensorDisplay;

namespace Spacelancer.Controllers;

public class UiController
{
    // Effectively forwards on the event subscription
    // May be bad if you care about the sender
    public event AutopilotMenu.AutopilotSelectedEventHandler AutopilotButtonSelected
    {
        add { _autopilotMenu.AutopilotSelected += value; }
        remove { _autopilotMenu.AutopilotSelected -= value; }
    }
    
    private SensorDisplay _sensorDisplay;
    private SensorViewport _sensorViewport;
    private AutopilotMenu _autopilotMenu;

    public void Initialize()
    {
        if (_sensorDisplay != null)
            return;
        
        _sensorDisplay = Global.GameController.LoadScene<SensorDisplay>("res://Scenes/UI/GameUI/Sensors/sensor_display.tscn");
        _sensorDisplay.Visible = false;

        _sensorViewport =
            Global.GameController.LoadScene<SensorViewport>("res://Scenes/UI/GameUI/Sensors/sensor_viewport.tscn");
        _sensorViewport.Visible = false;
        
        _autopilotMenu =
            Global.GameController.LoadScene<AutopilotMenu>("res://Scenes/UI/GameUI/Navigation/autopilot_menu.tscn");
        _autopilotMenu.Visible = false;
        
        Global.GameController.LoadScene<Control>("res://Scenes/UI/MainMenu/main_menu.tscn");
        
        Log.Debug("UiController initialized");
    }
    
    public void ShowSensorDisplay(bool visible = true) =>
        _sensorDisplay.Visible = visible;
    
    public void AddSensorDetection(ISensorDetectable detection) =>
        _sensorDisplay.AddItem(detection);
    
    public void RemoveSensorDetection(ulong id) =>
        _sensorDisplay.RemoveItem(id);

    public void ProcessNavigationSoftwareChange(NavigationSoftwareType softwareType) =>
        _autopilotMenu.SetActive(softwareType);

    public void SetSensorViewPortTarget(Node2D target)
    {
        _sensorViewport.SetCameraTarget(target);

        if (target is IDockable)
            _autopilotMenu.SetButtonAvailability(NavigationSoftwareType.Docking, true);
        else
            _autopilotMenu.SetButtonAvailability(NavigationSoftwareType.Docking, false);
        
        if (target is INavigable)
            _autopilotMenu.SetButtonAvailability(NavigationSoftwareType.Navigation, true);
        else
            _autopilotMenu.SetButtonAvailability(NavigationSoftwareType.Navigation, false);
    }


    public void ClearSensorViewPortTarget()
    {
        _sensorViewport.Visible = false;
        
        _autopilotMenu.SetButtonAvailability(NavigationSoftwareType.Docking, false);
        _autopilotMenu.SetButtonAvailability(NavigationSoftwareType.Navigation, false);
    }
    
    public void ShowAutopilotMenu(bool visible = true) =>
        _autopilotMenu.Visible = visible;
}