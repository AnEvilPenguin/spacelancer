using Godot;
using Serilog;
using Spacelancer.Components.Navigation;

namespace Spacelancer.Scenes.Transitions;

public partial class JumpGate : Node2D
{
    [Export]
    public string DestinationId {get; private set; }
    
    private static readonly PackedScene Scene = GD.Load<PackedScene>("res://Scenes/Transitions/jump_gate.tscn");
    
    private Area2D _entry;

    public static JumpGate GetNewInstance() =>
        Scene.Instantiate<JumpGate>();

    public override void _Ready()
    {
        _entry = GetNode<Area2D>("Area2D");

        _entry.BodyEntered += OnJumpBorderEntered;
        _entry.BodyExited += OnJumpBorderExited;
    }
    
    public Vector2 GetExitMarker() =>
        GetNode<Marker2D>("Exit").GlobalPosition;

    private void OnJumpBorderEntered(Node body)
    {
        Log.Debug("{Body} entered jump gate to {Destination}", body.Name, Name);
        
        if (body is Player.Player player)
        {
            TakeControlOfShip(player);
        }
    }

    private void OnJumpBorderExited(Node body)
    {
        Log.Debug("{Body} exited jump gate from {Destination}", body.Name, Name);
    }
    
    private void TakeControlOfShip(Player.Player player)
    {
        if (player.NavComputer is not PlayerNavigation)
            return;
		
        var computer = new JumpNavigation(player, this, Name);
        player.NavComputer = computer;
    }
}