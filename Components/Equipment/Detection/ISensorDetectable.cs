using Godot;

namespace Spacelancer.Components.Equipment.Detection;

public interface ISensorDetectable
{
    // TODO make this whole thing better
    // affiliations, health, shield, etc., etc.
    // not just simple string
    
    public SensorDetection Detect();

    public ulong GetInstanceId();
}