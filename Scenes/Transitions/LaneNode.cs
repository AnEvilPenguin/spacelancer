using Godot;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Scenes.Transitions;

public partial class LaneNode : LanePart, ISensorDetectable
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
        GenerateRing(-offset, ringTexture, RingDirection.Pair1);
    }
    
    public string GetName(Vector2 detectorPosition)
    {
        var isPair1Closer = detectorPosition.DistanceTo(TowardsPair1.GlobalPosition) <
                            detectorPosition.DistanceTo(TowardsPair2.GlobalPosition);

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

    private void GenerateRing(Vector2 position, Texture2D texture, RingDirection direction)
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
    }
}