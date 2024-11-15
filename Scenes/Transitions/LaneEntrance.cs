using Godot;
using Spacelancer.Components.Navigation;

namespace Spacelancer.Scenes.Transitions;

public partial class LaneEntrance : LanePart
{
    public override Node2D TowardsPair1 { get; set; }
    public override Node2D TowardsPair2 { get; set; }

    public LaneEntrance Partner;

    private Vector2 _lightOffset = new (110, 0);

    private Node2D _entrance;
    private Node2D _exit;
    
    public override string GetName(Node2D caller)
    {
        return Name;
    }
    
    public LaneEntrance(Vector2 position, Vector2 offset, Texture2D mainTexture, Texture2D goLight, Texture2D stopLight)
    {
        Position = position;
        
        _entrance = GenerateMainNode(offset, mainTexture, goLight, "Entrance");
        _exit = GenerateMainNode(-offset, mainTexture, stopLight, "Exit");
        
        GenerateDockingArea(_entrance);
    }
    
    // Required for the editor
    public LaneEntrance() {}
    
    public Node2D GetExitNode() => 
        _exit;

    private Node2D GenerateMainNode(Vector2 position, Texture2D texture, Texture2D light, string name)
    {
        var newNode = new Node2D();
        newNode.Position = position;
        newNode.Name = name;

        var sprite = new Sprite2D();
        sprite.Texture = texture;
		
        newNode.AddChild(sprite);
        
        GenerateLights(newNode, _lightOffset, light);
		
        AddChild(newNode);
        return newNode;
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

    private void GenerateDockingArea(Node2D node)
    {
        var dockingArea = new Area2D();

        var collisionShape = new CollisionPolygon2D();
        collisionShape.Polygon = new Vector2[] {
            new (-123, 0),
            new (123, 0),
            new (0, -123)
        };
        
        dockingArea.AddChild(collisionShape);
        node.AddChild(dockingArea);

        dockingArea.BodyEntered += OnLaneBorderEntered;
    }
    
    private void OnLaneBorderEntered(Node2D body)
    {
        // Log.Debug("{Body} entered space lane {Origin} to {Destination}", body.Name, Name, Partner.Name);

        if (body is Player.Player player)
        {
            TakeControlOfShip(player);
        }
    }

    private void TakeControlOfShip(Player.Player player)
    {
        if (player.NavComputer is not PlayerNavigation)
            return;
		
        // FIXME deal with exiting the lane and clear up old lanes
        
        var computer = new LaneNavigation(player, _entrance, Partner.GetExitNode());
        player.NavComputer = computer;
    }
}