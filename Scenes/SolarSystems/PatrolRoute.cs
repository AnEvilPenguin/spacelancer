using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Spacelancer.Components.Navigation.Software;
using Spacelancer.Scenes.Stations;

namespace Spacelancer.Scenes.SolarSystems;

public partial class PatrolRoute : Route
{
    [ExportGroup("Route")]
    [Export] 
    private Array<Vector2> _waypoints;
    
    [Export]
    private bool _reverseOnComplete;

    [Export] 
    private Station _homeStation;

    private bool _init = false;
    
    private List<Vector2> _route;

    public override void _Ready()
    {
        if (_waypoints is null || _waypoints.Count == 0)
            _route = GetChildren()
                .OfType<Marker2D>()
                .Select(m => m.GlobalPosition)
                .ToList();
        else
            _route = _waypoints.ToList();
        
        base._Ready();
    }

    public override void _Process(double delta)
    {
        if (_init)
            return;
        
        OnTimerTimeout();
        _init = true;
    }

    protected override void OnTimerTimeout()
    {
        if (_nonPlayerCharacters.Count >= _maxInstances)
            return;
        
        var npc = getNpcShipInstance(GetParent().GetParent());

        if (_init && _homeStation != null)
            npc.GlobalPosition = _homeStation.GlobalPosition;
        else
            npc.GlobalPosition = _route[0];

        // TODO consider getting a stack of navigation if home station is far from patrol start
        var software = new PatrolNavigation($"{Name} - {_nonPlayerCharacters.Count}", _route, _reverseOnComplete);
        npc.NavComputer.SetAutomatedNavigation(software);
    }
}