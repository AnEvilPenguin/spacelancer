using Godot;

namespace Spacelancer.Scenes.Transitions;

public abstract partial class LanePart : Node2D
{
    public abstract LanePart TowardsPair1 { get; set; }
    public abstract LanePart TowardsPair2 { get; set; }

    public LanePart GetNextPart(RingDirection direction) =>
        direction == RingDirection.Pair1 ? TowardsPair1 : TowardsPair2;
    
    public abstract bool IsDisrupted { get; protected set; }
}