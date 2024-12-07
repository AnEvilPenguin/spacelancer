using Godot;

namespace Spacelancer.Components.Equipment.Detection;

// TODO think about how we actually want this to operate
public partial class IdentificationFriendFoe : Area2D, ISensorDetectable
{
    private Node2D _parent;
    private readonly SensorDetection _detection;
    
    public IdentificationFriendFoe(Node2D parent, SensorDetection detection)
    {
        _parent = parent;
        parent.AddChild(this);
        
        _detection = detection;
        
        var collisionShape2D = new CollisionShape2D();
        var circleShape = new CircleShape2D();
        
        circleShape.Radius = 1;
        collisionShape2D.Shape = circleShape;
        
        AddChild(collisionShape2D);
    }

    // Required for editor
    private IdentificationFriendFoe() {}

    public SensorDetection Detect() =>
        _detection;
}