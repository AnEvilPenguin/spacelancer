using Serilog;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Equipment.Storage;
using Spacelancer.Components.Navigation;
using Spacelancer.Components.Navigation.Computers;
using Spacelancer.Components.Navigation.Software;
using Spacelancer.Controllers;
using Spacelancer.Economy;

namespace Spacelancer.Scenes.SpaceShips;

public partial class Player
{
    public override AbstractNavigationComputer NavComputer => _navComputer;

    private PlayerNavComputer _navComputer;
    public INavigationSoftware NavSoftware => NavComputer.GetCurrentSoftware();

    public int Credits = 500;

    protected override Sensor Sensor { get; set; }
    protected override IdentificationFriendFoe IFF { get; set; }
    
    public void SetTarget(ulong id)
    {
        Sensor.LockTarget(id);

        var detection = Sensor.GetLockedTarget();

        if (detection == null)
        {
            Log.Debug("Target with id '{id}' not found", id);
            return;
        }
            
        SetPointerTarget(detection.Body);
        _navComputer.ProcessNewTarget(detection.Body);
        Global.UserInterface.SetSensorViewPortTarget(detection.Body);
    }

    public void ClearTarget()
    {
        Sensor.ClearLockedTarget();
        
        ClearPointerTarget();
        _navComputer.ClearTarget();
        Global.UserInterface.ClearSensorViewPortTarget();
    }
    
    public SensorDetection GetTarget() =>
        Sensor.GetLockedTarget();

    private void SetDefaultEquipment()
    {
        PlayerNavigation defaultNavSoftware = new (this);
        _navComputer = new PlayerNavComputer(defaultNavSoftware);
        
        Sensor = new Sensor(10_000f);
        AddChild(Sensor);

        Sensor.SensorDetection += (sender, args) =>
            Global.UserInterface.AddSensorDetection(args.Detection);

        Sensor.SensorLost += (sender, args) =>
            Global.UserInterface.RemoveSensorDetection(args.Id);

        var detection = new SensorDetection(GetInstanceId(), "Player", "Temp", SensorDetectionType.Ship, this);
        IFF = new IdentificationFriendFoe(this, detection);
        
        Hold = new CargoHold(CommoditySize.Medium, 100);
    }
}