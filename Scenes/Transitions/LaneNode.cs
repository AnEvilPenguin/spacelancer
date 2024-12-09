using System.Linq;
using Godot;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Navigation;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Scenes.Transitions;

public partial class LaneNode : LanePart, ISensorDetectable, IDockable
{
    public override LanePart TowardsPair1 { get; set; }
    public override LanePart TowardsPair2 { get; set; }
    public override bool IsDisrupted { get; protected set; }
    
    private IdentificationFriendFoe _iff;

    public SensorDetectionType ReturnType => 
        SensorDetectionType.SpaceLaneNode;

    public string Affiliation =>
        "Unaffiliated";

    public LaneNode(Vector2 position, Texture2D ringTexture, Vector2 offset)
    {
        Position = position;
        
        _iff = new IdentificationFriendFoe(this);
        AddChild(_iff);
        
        GenerateMarker();
        
        GenerateRing(offset, ringTexture, RingDirection.Pair2);
        GenerateRing(-offset, ringTexture, RingDirection.Pair1)
            .RotationDegrees = 180;
    }

    public Marker2D GetNearestMarker(Vector2 position)
    {
        // Inverted. If we're closer to pair1 we're traveling towards pair 2
        var direction = IsPair1Closer(position) ? RingDirection.Pair2 : RingDirection.Pair1;
        
        var ring = GetChildren().OfType<LaneRing>().First(r => r.Direction == direction);

        return ring.GetNode<Marker2D>("Navigation Marker");
    }

    public string GetName(Vector2 detectorPosition)
    {
        var isPair1Closer = IsPair1Closer(detectorPosition);

        LanePart endpoint = this;
        bool shouldContinue = true;
        
        do
        {
            var next = endpoint.GetNextPart(isPair1Closer);

            if (next == null)
                shouldContinue = false;
            else
                endpoint = next;
            
        } while (shouldContinue);

        return endpoint.Name;
    }

    public Node2D ToNode2D() =>
        this as Node2D;
    
    // Required for the editor
    private LaneNode() {}
    
    private void GenerateMarker()
    {
        var marker = new Marker2D();
        
        AddChild(marker);
    }

    private LaneRing GenerateRing(Vector2 position, Texture2D texture, RingDirection direction)
    {
        var ring = new LaneRing(position, texture, direction);

        // TODO health and actually disrupt our rings
        ring.Area2D.BodyEntered += (Node2D body) =>
        {
            if (body is not SpaceShips.Player player)
                return;
            
            if (player.NavSoftware is not LaneNavigation navigation)
                return;
            
            bool nextRingDisrupted = direction == RingDirection.Pair1 ?
                    TowardsPair1.IsDisrupted : TowardsPair2.IsDisrupted;
            
            if (!nextRingDisrupted)
                return;
            
            navigation.DisruptTravel();
        };
        
        AddChild(ring);
        return ring;
    }

    public string Id =>
        "Temp";
    public AutomatedNavigation GetDockComputer()
    {
        throw new System.NotImplementedException();
    }
    
    private bool IsPair1Closer(Vector2 position) =>
        position.DistanceTo(TowardsPair1.GlobalPosition) <
        position.DistanceTo(TowardsPair2.GlobalPosition);
}