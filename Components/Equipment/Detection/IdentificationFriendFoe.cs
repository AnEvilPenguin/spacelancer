using Godot;

namespace Spacelancer.Components.Equipment.Detection;

// TODO think about how we actually want this to operate
public partial class IdentificationFriendFoe : Area2D
{
    private readonly ISensorDetectable _parent;

    private readonly ulong _instanceId;
    private readonly string _affiliation;
    private readonly SensorDetectionType _returnType;
    private readonly Node2D _body;
    
    public IdentificationFriendFoe(ISensorDetectable parent)
    {
        _parent = parent;

        _instanceId = parent.GetInstanceId();
        _affiliation = parent.Affiliation;
        _returnType = parent.ReturnType;
        _body = parent.ToNode2D();
        
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