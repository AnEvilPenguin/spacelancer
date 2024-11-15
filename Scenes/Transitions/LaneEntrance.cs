using Godot;

namespace Spacelancer.Scenes.Transitions;

public partial class LaneEntrance : LanePart
{
    public override Node2D TowardsPair1 { get; set; }
    public override Node2D TowardsPair2 { get; set; }

    private Vector2 _lightOffset = new (110, 0);
    public override string GetName(Node2D caller)
    {
        return Name;
    }
    
    public LaneEntrance(Vector2 position, Vector2 offset, Texture2D mainTexture, Texture2D goLight, Texture2D stopLight)
    {
        Position = position;
        
        GenerateMainNode(offset, mainTexture, goLight);
        GenerateMainNode(-offset, mainTexture, stopLight);
    }
    
    // Required for the editor
    public LaneEntrance() {}

    private void GenerateMainNode(Vector2 position, Texture2D texture, Texture2D light)
    {
        var newNode = new Node2D();
        newNode.Position = position;

        var sprite = new Sprite2D();
        sprite.Texture = texture;
		
        newNode.AddChild(sprite);
        
        GenerateLights(newNode, _lightOffset, light);
		
        AddChild(newNode);
    }
    
    private void GenerateLights(Node2D node, Vector2 position, Texture2D lightTexture)
    {
        node.AddChild(GenerateLight(position, lightTexture));
        node.AddChild(GenerateLight(-position, lightTexture));
    }

    private Sprite2D GenerateLight(Vector2 position, Texture2D texture)
    {
        var sprite = new Sprite2D();
        sprite.Texture = texture;
        sprite.ZIndex = -2;
        sprite.Position = position;
		
        return sprite;
    }
}