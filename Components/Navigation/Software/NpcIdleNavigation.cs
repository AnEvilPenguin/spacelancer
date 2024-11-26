using Godot;

namespace Spacelancer.Components.Navigation.Software;

public class NpcIdleNavigation : INavigationSoftware
{
    public string Name => "NpcIdleNavigation";
    public NavigationSoftwareType Type => NavigationSoftwareType.Manual;
    public float GetRotation(float maxRotation, float currentAngle, Vector2 currentVelocity) => 
        currentVelocity.Angle();

    public Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity) =>
        currentVelocity;
}