using Godot;

namespace Spacelancer.Scenes.Transitions;

public abstract partial class LanePart : Node2D
{
    public abstract Node2D TowardsPair1 { get; set; }
    public abstract Node2D TowardsPair2 { get; set; }
    
    public abstract string GetName(Node2D caller);
}