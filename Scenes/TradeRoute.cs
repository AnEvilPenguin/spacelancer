using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Godot;
using Spacelancer.Components.Navigation;
using Spacelancer.Components.Navigation.Software;
using Spacelancer.Scenes.SpaceShips;
using Spacelancer.Scenes.Transitions;

namespace Spacelancer.Scenes;

public partial class TradeRoute : Node
{
    [ExportGroup("Timings")] 
    [Export] 
    private float _minTimerSeconds = 30;
    
    [Export]
    private float _maxTimerSeconds = 300;
    
    [ExportGroup("Route")]
    [Export]
    private PackedScene _nonPlayerCharacterScene;

    [Export] 
    private Array<Node2D> _route;
    
    [Export]
    private bool _biDirectional;

    [Export] 
    private int _maxInstances = 2;
    
    
    private Timer _timer;
    private List<Node2D> _nonPlayerCharacters = new();
    private RandomNumberGenerator _rand = new();

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += OnTimerTimeout;
        
        _timer.Start(_minTimerSeconds);
    }

    private void OnTimerTimeout()
    {
        if (_nonPlayerCharacters.Count >= _maxInstances)
            return;
        
        var npc = _nonPlayerCharacterScene.Instantiate<InitialNpcShip>();
        
        npc.TreeExiting += () =>
        {
            _nonPlayerCharacters.Remove(npc);
            
            if (_timer.IsStopped())
                StartTimer();
        };
        
        npc.GlobalPosition = _route[0].GlobalPosition;
        
        var tradeRoute = GetTradeRoute(npc.GlobalPosition);
        
        _nonPlayerCharacters.Add(npc);
        GetParent().GetParent().AddChild(npc);
        
        npc.SetTradeRoute(tradeRoute);

        StartTimer();
    }
    
    private float GetTimerTimeout() =>
        _rand.RandfRange(_minTimerSeconds, _maxTimerSeconds);
    
    private void StartTimer() =>
        _timer.Start(GetTimerTimeout());

    // Assumes that a station or Jump gate is the last node
    private Stack<AutomatedNavigation> GetTradeRoute(Vector2 position)
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

                    var docker = entrance.GetDockComputer();
                    
                    output.Add(new SystemAutoNavigation(entrance));
                    output.Add(docker);

                    position = entrance.GetExitNode().GlobalPosition;
                    break;
                }
                
                case IDockable dockingObject:
                {
                    var docker = dockingObject.GetDockComputer();
                    
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