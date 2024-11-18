using Godot;

namespace Spacelancer.Components.Equipment.Detection;

public class SensorDetection
{
    public string Name { get; }
    public string Affiliation { get; }
    public SensorDetectionType ReturnType { get; }
    public Node2D Body { get; }

    public SensorDetection(string name, string affiliation, SensorDetectionType returnType, Node2D body)
    {
        Name = name;
        Affiliation = affiliation;
        ReturnType = returnType;
        Body = body;
    }
}