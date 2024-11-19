using Godot;
using Serilog;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Scenes.UI.GameUI;

namespace Spacelancer.Controllers;

public class UiController
{
    private SensorDisplay _sensorDisplay;

    public void Initialize()
    {
        if (_sensorDisplay != null)
            return;
        
        _sensorDisplay = Global.GameController.LoadScene<SensorDisplay>("res://Scenes/UI/GameUI/SensorDisplay.tscn");
        _sensorDisplay.Visible = false;
        
        Global.GameController.LoadScene<Control>("res://Scenes/UI/MainMenu/main_menu.tscn");
        
        Log.Debug("UiController initialized");
    }
    
    public void ShowSensorDisplay() =>
        _sensorDisplay.Visible = true;
    
    public void AddSensorDetection(SensorDetection detection) =>
        _sensorDisplay.AddItem(detection);
    
    public void RemoveSensorDetection(ulong id) =>
        _sensorDisplay.RemoveItem(id);
}