using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Serilog;

/// <summary>
/// Handles loading and displaying of scenes and user interfaces.
/// </summary>
public partial class GameController : Node
{
    // FIXME extract this into a proper component
    public Label TempStationLabel;
    
    private Control _gui;
    private Node2D _world2D;

    private Dictionary<string, Node> _loadedScenes = new Dictionary<string, Node>();
    private Dictionary<ulong, string> _nodeToPathLookup = new Dictionary<ulong, string>();

    public override void _Ready()
    {
        Global.GameController = this;

        _world2D = GetNode<Node2D>("World2D");
        _gui = GetNode<Control>("%GUI");
        
        TempStationLabel = _gui.GetNode<Label>("Comms Label");

        LoadScene<Control>("res://Scenes/UI/MainMenu/main_menu.tscn");
        
        Log.Debug("Game controller loaded");
    }

    public T LoadScene<T>(string scenePath) where T : Node
    {
        if (_loadedScenes.ContainsKey(scenePath) && _loadedScenes[scenePath] is T scene)
            return scene;

        var newScene = GD.Load<PackedScene>(scenePath);
        var instance = newScene.Instantiate<T>();
        
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

    private void UnloadScene(Node scene)
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

    // FIXME load, hide, unload scene logic
    // consider dictionary of loaded items, to help with decisions over what to do with existing scenes
}
