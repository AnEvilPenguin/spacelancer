using Godot;
using Serilog;

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

        LoadScene("res://Scenes/UI/MainMenu/main_menu.tscn", true);
        
        Log.Debug("Game controller loaded");
    }

    public void LoadScene(string scenePath, bool isUi = false)
    {
        CanvasItem parent = isUi ? _gui : _world2D;
        
        var newScene = GD.Load<PackedScene>(scenePath);
        var instance = newScene.Instantiate<Node>();

        parent.AddChild(instance);
    }
    
    // FIXME load, hide, unload scene logic
}
