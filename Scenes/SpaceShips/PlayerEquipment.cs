using Serilog;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Equipment.Storage;
using Spacelancer.Components.Navigation;
using Spacelancer.Controllers;
using Spacelancer.Economy;

namespace Spacelancer.Scenes.SpaceShips;

public partial class Player
{
    public CargoHold Hold;
    
    public PlayerNavComputer NavComputer;
    
    public INavigationSoftware NavSoftware => NavComputer.GetCurrentSoftware();

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
        NavComputer.ProcessNewTarget(detection.Body);
        Global.UserInterface.SetSensorViewPortTarget(detection.Body);
    }

    public void ClearTarget()
    {
        _sensor.ClearLockedTarget();
        
        ClearPointerTarget();
        NavComputer.ClearTarget();
        Global.UserInterface.ClearSensorViewPortTarget();
    }
    
    public SensorDetection GetTarget() =>
        _sensor.GetLockedTarget();

    public void ResetNavComputer() =>
        NavComputer.ResetNavSoftware();

    private void SetDefaultEquipment()
    {
        PlayerNavigation defaultNavSoftware = new (this);
        NavComputer = new PlayerNavComputer(defaultNavSoftware);
        
        _sensor = new Sensor(10_000f);
        AddChild(_sensor);

        _sensor.SensorDetection += (sender, args) =>
            Global.UserInterface.AddSensorDetection(args.Detection);

        _sensor.SensorLost += (sender, args) =>
            Global.UserInterface.RemoveSensorDetection(args.Id);

        var detection = new SensorDetection(GetInstanceId(), "Player", "Temp", SensorDetectionType.Ship, this);
        _iff = new IdentificationFriendFoe(this, detection);
        
        Hold = new CargoHold(CommoditySize.Medium, 100);
    }
}