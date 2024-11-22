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
    
    public float GetRotation(float maxRotation);
    
    public Vector2 GetVelocity(float maxSpeed);
}