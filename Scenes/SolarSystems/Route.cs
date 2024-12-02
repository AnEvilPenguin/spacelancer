using System.Collections.Generic;
using Godot;
using Spacelancer.Components.Navigation.Software;
using Spacelancer.Scenes.SpaceShips;

namespace Spacelancer.Scenes.SolarSystems;

public abstract partial class Route : Node
{
    [ExportGroup("Timings")] 
    [Export] 
    protected float _minTimerSeconds = 30;
    
    [Export]
    protected float _maxTimerSeconds = 300;
    
    [ExportGroup("Route")]
    [Export]
    protected PackedScene _nonPlayerCharacterScene;
    
    [Export] 
    protected int _maxInstances = 2;
    
    protected Timer _timer;
    protected List<Node2D> _nonPlayerCharacters = new();
    private RandomNumberGenerator _rand = new();

    protected abstract void OnTimerTimeout();
    
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += OnTimerTimeout;
        
        _timer.Start(_minTimerSeconds);
    }
    
    protected float GetTimerTimeout() =>
        _rand.RandfRange(_minTimerSeconds, _maxTimerSeconds);
    
    protected void StartTimer() =>
        _timer.Start(GetTimerTimeout());


    protected InitialNpcShip getNpcShipInstance(Node parent)
    {
        var npc = _nonPlayerCharacterScene.Instantiate<InitialNpcShip>();
        
        npc.TreeExiting += () =>
        {
            _nonPlayerCharacters.Remove(npc);
            
            if (_timer.IsStopped())
                StartTimer();
        };
        
        _nonPlayerCharacters.Add(npc);
        parent.AddChild(npc);
        
        return npc;
    }
}