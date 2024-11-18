using Godot;

namespace Spacelancer.Scenes.Transitions;

public enum RingDirection
{
    Pair1,
    Pair2
}

public partial class LaneRing : Node2D
{
    public Area2D Area2D;
    
    public RingDirection Direction;

    public LaneRing(Vector2 position, Texture2D texture, RingDirection direction)
    {
        Position = position;
        Direction = direction;
        
        var sprite = new Sprite2D();
        sprite.Texture = texture;
        AddChild(sprite);
        
        Area2D = new Area2D();
        var collisionShape = new CollisionShape2D();
        var rectangleShape = new RectangleShape2D();
        
        rectangleShape.Size = new Vector2(10, 50);
        collisionShape.Shape = rectangleShape;
        
        Area2D.AddChild(collisionShape);
        AddChild(Area2D);
    }
    
    // Needed for editor
    public LaneRing() {}
}