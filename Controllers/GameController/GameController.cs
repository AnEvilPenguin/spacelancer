using Godot;
using Serilog;

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
}
