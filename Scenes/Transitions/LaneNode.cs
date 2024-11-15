using Godot;

namespace Spacelancer.Scenes.Transitions;

public partial class LaneNode : LanePart
{
    public override Node2D TowardsPair1 { get; set; }
    public override Node2D TowardsPair2 { get; set; }
    public override string GetName(Node2D caller)
    {
        throw new System.NotImplementedException();
    }

    public LaneNode(Vector2 position, Texture2D ringTexture, Vector2 offset)
    {
        Position = position;
        
        GenerateMarker();
        
        GenerateRing(offset, ringTexture);
        GenerateRing(-offset, ringTexture);
    }

    // Potentially required for Godot?
    public LaneNode()
    {
        throw new System.NotImplementedException();
    }

    private void GenerateMarker()
    {
        var marker = new Marker2D();
        
        AddChild(marker);
    }

    private void GenerateRing(Vector2 position, Texture2D texture)
    {
        var ring = new Node2D();
        ring.Position = position;

        var sprite = new Sprite2D();
        sprite.Texture = texture;
        ring.AddChild(sprite);
        
        AddChild(ring);
    }
}