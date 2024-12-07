using System.Collections.Generic;
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
        _npcNavComputer.Jumping += (_, _) => RemoveShip();
        _npcNavComputer.Complete += (_, _) => RemoveShip();
        _npcNavComputer.Docking += (_, _) => RemoveShip();
        
        IFF = new IdentificationFriendFoe(this, new SensorDetection(GetInstanceId(), "NPC Test", "Unaffiliated", SensorDetectionType.Ship, this));
        
        Sensor = new Sensor(10_000f);
        AddChild(Sensor);
    }

    public override void _Process(double delta)
    {
        if (NavComputer.GetCurrentSoftware() is not NpcIdleNavigation || string.IsNullOrWhiteSpace(_destination))
            return;
        
        var stack = Global.SolarSystem.GetAutomatedRoute(GlobalPosition, _destination);

        var first = stack.Pop();
        NavComputer.SetAutomatedNavigation(first);
        
        NavComputer.SetNavigationStack(stack);
    }

    public void SetRoute(Stack<AutomatedNavigation> stack) =>
        NavComputer.SetNavigationStack(stack);

    private void RemoveShip()
    {
        // Evaluate if this is good enough in general
        Visible = false;
        QueueFree();
    }
}