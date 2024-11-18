using Godot;
using Serilog;
using Spacelancer.Components.Navigation;

namespace Spacelancer.Scenes.Transitions;

public partial class LaneNode : LanePart
{
    public override LanePart TowardsPair1 { get; set; }
    public override LanePart TowardsPair2 { get; set; }
    public override bool IsDisrupted { get; protected set; }

    public override string GetName(Node2D caller)
    {
        throw new System.NotImplementedException();
    }

    public LaneNode(Vector2 position, Texture2D ringTexture, Vector2 offset)
    {
        Position = position;
        
        GenerateMarker();
        
        GenerateRing(offset, ringTexture, RingDirection.Pair2);
        GenerateRing(-offset, ringTexture, RingDirection.Pair1);
    }
    
    // Required for the editor
    public LaneNode() {}

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
            if (body is not Player.Player player)
                return;
            
            if (player.NavComputer is not LaneNavigation navigation)
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