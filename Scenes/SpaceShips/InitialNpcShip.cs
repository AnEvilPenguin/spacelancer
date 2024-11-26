using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Navigation.Computers;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Scenes.SpaceShips;

public partial class InitialNpcShip : Ship
{
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
}