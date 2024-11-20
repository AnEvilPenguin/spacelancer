using Godot;
using Serilog;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Equipment.Storage;
using Spacelancer.Components.Navigation;
using Spacelancer.Controllers;
using Spacelancer.Economy;

namespace Spacelancer.Scenes.Player;

public partial class Player
{
    public CargoHold Hold;
    
    public INavigationSoftware NavComputer;

    public int Credits = 500;

    private Sensor _sensor;

    private IdentificationFriendFoe _iff;
    
    

    public void SetTarget(ulong id)
    {
        _sensor.LockTarget(id);

        var detection = _sensor.GetLockedTarget();

        if (detection == null)
        {
            Log.Debug("Target with id '{id}' not found", id);
            return;
        }
            
        SetPointerTarget(detection.Body);
        Global.UserInterface.SetSensorViewPortTarget(detection.Body);
    }

    public void ClearTarget()
    {
        _sensor.ClearLockedTarget();
        
        ClearPointerTarget();
        Global.UserInterface.ClearSensorViewPortTarget();
    }
    
    public SensorDetection GetTarget() =>
        _sensor.GetLockedTarget();

    private void SetDefaultEquipment()
    {
        _sensor = new Sensor(10_000f);
        AddChild(_sensor);

        _sensor.SensorDetection += (sender, args) =>
            Global.UserInterface.AddSensorDetection(args.Detection);

        _sensor.SensorLost += (sender, args) =>
            Global.UserInterface.RemoveSensorDetection(args.Id);

        var detection = new SensorDetection(GetInstanceId(), "Player", "Temp", SensorDetectionType.Ship, this);
        _iff = new IdentificationFriendFoe(this, detection);
        
        Hold = new CargoHold(CommoditySize.Medium, 100);
        NavComputer = new PlayerNavigation(this);
    }
}