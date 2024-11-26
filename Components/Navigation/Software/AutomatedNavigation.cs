using System;
using Godot;

namespace Spacelancer.Components.Navigation.Software;

public abstract class AutomatedNavigation : INavigationSoftware
{   
    public abstract event EventHandler Complete;
    public abstract event EventHandler Aborted;

    public abstract void DisruptTravel();
    
    public abstract string Name { get; }
    public abstract NavigationSoftwareType Type { get; }
    public abstract float GetRotation(float maxRotation, float currentAngle, Vector2 currentVelocity);

    public abstract Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity);
    
    protected void RaiseEvent(EventHandler handler)
    {
        var raiseEvent = handler;
        raiseEvent?.Invoke(this, EventArgs.Empty);
    }
}