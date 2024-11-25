using System.Collections.Generic;
using System.Linq;
using Godot;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Navigation;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Scenes.Transitions;

public partial class LaneEntrance : LanePart, IDockable
{
    public override LanePart TowardsPair1 { get; set; }
    public override LanePart TowardsPair2 { get; set; }
    public override bool IsDisrupted { get; protected set; }

    public LaneEntrance Partner;

    private Vector2 _lightOffset = new (110, 0);

    private Node2D _entrance;
    private Node2D _exit;
    
    private IdentificationFriendFoe _iff;

    private List<Marker2D> _markers = new();
    
    public LaneEntrance(Vector2 position, Vector2 offset, Texture2D mainTexture, Texture2D goLight, Texture2D stopLight)
    {
        Position = position;
        
        var detection = new SensorDetection(GetInstanceId(), Name, "Unaffiliated", SensorDetectionType.SpaceLane, this);
        _iff = new IdentificationFriendFoe(this, detection);
        
        _entrance = GenerateMainNode(offset, mainTexture, goLight, "Entrance");
        _exit = GenerateMainNode(-offset, mainTexture, stopLight, "Exit");
        
        GenerateDockingArea(_entrance);
        GenerateMarkers();
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

    private void GenerateMarkers()
    {
        _markers.ForEach(m => m.QueueFree());
        _markers.Clear();
        
        _markers.Add(GenerateMarker(new Vector2(300, -300), "Top"));
        _markers.Add(GenerateMarker(new Vector2(0, -300), "Middle"));
        _markers.Add(GenerateMarker(new Vector2(-300, -300), "Bottom"));
    }

    private Marker2D GenerateMarker(Vector2 position, string name)
    {
        var marker = new Marker2D();
        marker.Name = name;
        marker.Position = position;
        
        AddChild(marker);
        return marker;
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

        if (body is SpaceShips.Player player)
        {
            TakeControlOfShip(player);
        }
    }

    private void TakeControlOfShip(SpaceShips.Player player)
    {
        if (player.NavSoftware is not PlayerNavigation)
            return;
        
        var computer = new LaneNavigation(_entrance, Partner.GetExitNode());
        player.NavComputer.SetAutomatedNavigation(computer);
    }

    public Marker2D GetNearestMarker(Vector2 position) =>
        _markers.Aggregate((acc, cur) =>
        {
            var dist1 = (position - cur.GlobalPosition).Length();
            var dist2 = (position - acc.GlobalPosition).Length();
			
            return dist1 < dist2 ? cur : acc;
        });

    public string GetName(Vector2 _) =>
        Name;

    public AutomatedNavigation GetDockComputer() =>
        new LaneNavigation(_entrance, Partner.GetExitNode());
}