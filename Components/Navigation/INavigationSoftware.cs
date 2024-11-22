using Godot;

namespace Spacelancer.Components.Navigation;

public enum NavigationSoftwareType
{
    Manual,
    Navigation,
    Docking,
    Formation,
}

public interface INavigationSoftware
{
    public string Name { get; }
    public NavigationSoftwareType Type { get; }
    
    // FIXME consider taking in current rotation/position(?) to break dependency on ship
    public float GetRotation(float maxRotation);
    
    // FIXME consider taking in current velocity/position to break dependency on ship
    public Vector2 GetVelocity(float maxSpeed);
}