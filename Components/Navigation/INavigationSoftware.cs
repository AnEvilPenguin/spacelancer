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
    
    public float GetRotation(float maxRotation, float currentAngle, Vector2 currentVelocity);
    
    public Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity);
}