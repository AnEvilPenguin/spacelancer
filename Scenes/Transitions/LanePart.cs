using Godot;

namespace Spacelancer.Scenes.Transitions;

public abstract partial class LanePart : Node2D
{
    public abstract LanePart TowardsPair1 { get; set; }
    public abstract LanePart TowardsPair2 { get; set; }

    public abstract bool IsDisrupted { get; protected set; }
    
    public abstract string GetName(Node2D caller);
}