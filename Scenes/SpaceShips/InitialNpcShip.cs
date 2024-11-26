using Godot;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Navigation.Computers;
using Spacelancer.Components.Navigation.Software;
using Spacelancer.Controllers;

namespace Spacelancer.Scenes.SpaceShips;

public partial class InitialNpcShip : Ship
{
    [Export] 
    private string _destination = "UA02";
    
    public override AbstractNavigationComputer NavComputer =>
        _npcNavComputer;
    protected override IdentificationFriendFoe IFF { get; set; }
    protected override Sensor Sensor { get; set; }
    
    private NpcNavComputer _npcNavComputer;

    public override void _Ready()
    {
        _npcNavComputer = new NpcNavComputer(new NpcIdleNavigation());
        IFF = new IdentificationFriendFoe(this, new SensorDetection(GetInstanceId(), "NPC Test", "Unaffiliated", SensorDetectionType.Ship, this));
        
        Sensor = new Sensor(10_000f);
        AddChild(Sensor);
    }

    public override void _Process(double delta)
    {
        if (NavComputer.GetCurrentSoftware() is not NpcIdleNavigation)
            return;
        
        var comp = Global.SolarSystem.CalculateBestRoute(GlobalPosition, _destination);
        
        NavComputer.SetAutomatedNavigation(comp);
        
        // TODO call out to system?
        // Give it our position
        // Get list of computers required to get to destination?
    }
}