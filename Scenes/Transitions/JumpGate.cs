using System.Collections.Generic;
using System.Linq;
using Godot;
using Serilog;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Navigation;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Scenes.Transitions;

public partial class JumpGate : Node2D, IDockable, ISensorDetectable
{
    [Export]
    public string Id { get; private set; }
    
    public SensorDetectionType ReturnType =>
        SensorDetectionType.JumpGate;
    public string Affiliation =>
        "Unaffiliated";

    public string GetName(Vector2 _) =>
        $"{Name} Jump Gate";
    
    private static readonly PackedScene Scene = GD.Load<PackedScene>("res://Scenes/Transitions/jump_gate.tscn");
    
    private Area2D _entry;

    private IdentificationFriendFoe _iff;

    private List<Marker2D> _markers;

    public static JumpGate GetNewInstance() =>
        Scene.Instantiate<JumpGate>();

    public override void _Ready()
    {
        _entry = GetNode<Area2D>("Area2D");

        _entry.BodyEntered += OnJumpBorderEntered;
        _entry.BodyExited += OnJumpBorderExited;
        
        _iff = new IdentificationFriendFoe(this);
        AddChild(_iff);
        
        _markers = GetNode("Markers").GetChildren().OfType<Marker2D>().ToList();
    }
    
    public Vector2 GetExitMarker() =>
        GetNode<Marker2D>("Exit").GlobalPosition;
        

    private void OnJumpBorderEntered(Node body)
    {
        Log.Debug("{Body} entered jump gate to {Destination}", body.Name, Name);
    }

    private void OnJumpBorderExited(Node body)
    {
        Log.Debug("{Body} exited jump gate from {Destination}", body.Name, Name);
    }

    public Marker2D GetNearestMarker(Vector2 position) =>
        _markers.Aggregate((acc, cur) =>
        {
            var dist1 = (position - cur.GlobalPosition).Length();
            var dist2 = (position - acc.GlobalPosition).Length();
			
            return dist1 < dist2 ? cur : acc;
        });

    public Node2D ToNode2D() =>
        this as Node2D;

    public AutomatedNavigation GetDockComputer(Vector2 _) =>
        new JumpEntranceNavigation(this, Name);
}