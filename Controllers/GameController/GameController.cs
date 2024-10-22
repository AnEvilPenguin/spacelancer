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

        Log.Debug("Game controller loaded");
    }

    // FIXME load, hide, unload scene logic
    // FIXME auto load main menu scene in first on ready.
}
