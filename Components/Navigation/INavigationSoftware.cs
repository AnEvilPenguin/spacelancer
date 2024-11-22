using Godot;

namespace Spacelancer.Components.Navigation;

public interface INavigationSoftware
{
    public string Name { get; }
    
    public float GetRotation(float maxRotation);
    
    public Vector2 GetVelocity(float maxSpeed);
}