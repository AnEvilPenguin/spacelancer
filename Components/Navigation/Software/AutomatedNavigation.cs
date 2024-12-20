using System;
using Godot;
using Spacelancer.Components.Equipment.Detection;

namespace Spacelancer.Components.Navigation.Software;

public abstract class AutomatedNavigation : INavigationSoftware
{   
    // FIXME single Complete event with an enum for what type it is to stop compiler whinging
    // Could then have a jump specific one inheriting from that
    public abstract event EventHandler<NavigationCompleteEventArgs> Complete;

    public abstract void DisruptTravel();
    
    public abstract string Name { get; }
    public abstract NavigationSoftwareType Type { get; }
    public abstract float GetRotation(float maxRotation, float currentAngle, Vector2 currentVelocity);

    public abstract Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity);
    
    protected void RaiseEvent(EventHandler<NavigationCompleteEventArgs> handler, NavigationCompleteEventArgs e)
    {
        var raiseEvent = handler;
        raiseEvent?.Invoke(this, e);
    }
}