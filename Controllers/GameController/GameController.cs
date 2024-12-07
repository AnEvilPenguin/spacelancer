using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Serilog;
using Spacelancer.Scenes.SolarSystems;
using Spacelancer.Scenes.SpaceShips;
using Spacelancer.Scenes.Stations;

namespace Spacelancer.Controllers;

/// <summary>
/// Handles loading and displaying of scenes and user interfaces.
/// </summary>
public partial class GameController : Node
{
    private Control _gui;
    private Node2D _world2D;

    private Dictionary<string, Node> _loadedScenes = new ();
    private Dictionary<ulong, string> _nodeToPathLookup = new ();
    
    private BaseSystem _currentSystem;

    public override void _Ready()
    {
        Global.GameController = this;

        _world2D = GetNode<Node2D>("World2D");
        _gui = GetNode<Control>("%GUI");
        
        Global.UserInterface.Initialize();
        
        Global.Universe.LoadSystemScenes();
        
        Log.Debug("Game controller loaded");
    }

    public void NewGame()
    {
        UnloadWorld2D();
        
        Global.Economy.LoadEconomy();
        
        Global.Player = LoadScene<Player>("res://Scenes/SpaceShips/player.tscn");
        
        Global.UserInterface.ShowSensorDisplay();
        Global.UserInterface.ShowAutopilotMenu();
        
        var system = LoadSystem("UA01");
        
        var station = system.GetChildren().OfType<Station>().FirstOrDefault();
        if (station != null)
            Global.Player.Position = station.Position;
    }

    public BaseSystem LoadSystem(string systemId)
    {
        var newSystem = Global.Universe.GetSystemInstance(systemId);

        if (_currentSystem != null)
        {
            _world2D.RemoveChild(_currentSystem);
            _currentSystem.QueueFree();
        }
        
        _currentSystem = newSystem;
        Global.SolarSystem.CurrentSystem = _currentSystem;
        
        _world2D.AddChild(newSystem);
        
        return _currentSystem; 
    }

    public T LoadScene<T>(string scenePath) where T : Node
    {
        if (_loadedScenes.ContainsKey(scenePath) && _loadedScenes[scenePath] is T scene)
        {
            if (IsInstanceValid(scene) && !scene.IsQueuedForDeletion())
                return scene;
            
            _loadedScenes.Remove(scenePath);
        }
        
        T instance;

        try
        {
            var newScene = GD.Load<PackedScene>(scenePath);
            instance = newScene.Instantiate<T>();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error loading scene from {Path}", scenePath);
            throw;
        }
        
        _loadedScenes.Add(scenePath, instance);
        _nodeToPathLookup.Add(instance.GetInstanceId(), scenePath);
     
        Node parent = instance is Control ? _gui : _world2D;
        
        parent.AddChild(instance);
        return instance;
    }

    public void UnloadWorld2D()
    {
        var childNodes = _world2D.GetChildren()
            .Where(c => c != null)
            .ToList();
            
        Log.Debug("Unloading {nodeCount} from World2D", childNodes.Count);

        foreach (var child in childNodes)
        {
            try
            {
                UnloadScene(child);
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to unload {nodeName} - {nodeId}", child.Name, child.GetInstanceId());
            }
        }
    }

    public void UnloadScene(Node scene)
    {
        var id = scene.GetInstanceId();
        Log.Debug("Unloading {nodeName} - {nodeId}", scene.Name, id);

        if (_nodeToPathLookup.ContainsKey(id))
        {
            var scenePath = _nodeToPathLookup[id];
            _loadedScenes.Remove(scenePath);
            _nodeToPathLookup.Remove(id);
        }
        
        scene.QueueFree();
    }
    
    public BaseSystem GetCurrentSystem() =>
        _currentSystem;

    // FIXME load, hide, unload scene logic
    // consider dictionary of loaded items, to help with decisions over what to do with existing scenes
}