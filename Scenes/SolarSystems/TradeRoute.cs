using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Spacelancer.Components.Navigation;
using Spacelancer.Components.Navigation.Software;
using Spacelancer.Scenes.Transitions;

namespace Spacelancer.Scenes.SolarSystems;

public partial class TradeRoute : Route
{
    [ExportGroup("Route")]
    [Export] 
    private Array<Node2D> _route;

    [Export] 
    private bool _biDirectional;
    
    protected override void OnTimerTimeout()
    {
        if (_nonPlayerCharacters.Count >= _maxInstances)
            return;

        var npc = getNpcShipInstance(GetParent().GetParent());

        var route = _route.ToList();
        
        if (_biDirectional && GD.Randi() % 2 == 0)
            route.Reverse();
        
        npc.GlobalPosition = route[0].GlobalPosition;
        
        var tradeRoute = GetRoute(npc.GlobalPosition);
        npc.SetRoute(tradeRoute);

        StartTimer();
    }
    
    // Assumes that a station or Jump gate is the last node
    private Stack<AutomatedNavigation> GetRoute(Vector2 position)
    {
        var output = new List<AutomatedNavigation>();
        
        var nodes = _route.ToList();
        nodes.RemoveAt(0);

        nodes.ForEach(n =>
        {
            switch (n)
            {
                case SpaceLane lane:
                {
                    var entrance = lane.GetNearestEntrance(position);

                    var docker = entrance.GetDockComputer(position);
                    
                    output.Add(new SystemAutoNavigation(entrance));
                    output.Add(docker);

                    position = entrance.GetExitNode().GlobalPosition;
                    break;
                }
                
                case IDockable dockingObject:
                {
                    var docker = dockingObject.GetDockComputer(position);
                    
                    output.Add(new SystemAutoNavigation(dockingObject));
                    output.Add(docker);
                    
                    break;
                }
            }
        });

        output.Reverse();
        
        return new Stack<AutomatedNavigation>(output);
    }
}