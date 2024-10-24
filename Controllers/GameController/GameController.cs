using Godot;
using Serilog;

public enum SceneType
{
    Gui,
    World2D
}

/// <summary>
/// Handles loading and displaying of scenes and user interfaces.
/// </summary>
public partial class GameController : Node
{
    private Control _gui;
    private Node2D _world2D;

    public override void _Ready()
    {
        Global.Instance.GameController = this;

        _world2D = GetNode<Node2D>("World2D");
        _gui = GetNode<Control>("GUI");

        LoadScene("res://Scenes/UI/MainMenu/main_menu.tscn", SceneType.Gui);
        
        Log.Debug("Game controller loaded");
    }

    public void LoadScene(string scenePath, SceneType sceneType = SceneType.World2D)
    {
        CanvasItem parent = sceneType == SceneType.Gui ? _gui : _world2D;
        
        var newScene = GD.Load<PackedScene>(scenePath);
        var instance = newScene.Instantiate<Node>();

        parent.AddChild(instance);
    }
    
    // FIXME load, hide, unload scene logic
    // consider dictionary of loaded items, to help with decisions over what to do with existing scenes
}
