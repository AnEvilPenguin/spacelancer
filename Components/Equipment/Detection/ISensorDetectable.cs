using Godot;

namespace Spacelancer.Components.Equipment.Detection;

public interface ISensorDetectable
{
    // TODO make this whole thing better
    // affiliations, health, shield, etc., etc.
    // not just simple string

    public SensorDetectionType ReturnType { get; }
    public string Affiliation { get; }
    public ulong GetInstanceId();
    
    public string GetName(Vector2 detectorPosition);
    
    public Node2D ToNode2D();
}