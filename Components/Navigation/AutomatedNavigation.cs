using System;
using Godot;

namespace Spacelancer.Components.Navigation;

public abstract class AutomatedNavigation : INavigationSoftware
{   
    public abstract event EventHandler Complete;
    public abstract event EventHandler Aborted;

    public abstract void DisruptTravel();
    
    public abstract string Name { get; }
    public abstract NavigationSoftwareType Type { get; }
    public abstract float GetRotation(float maxRotation);
    public abstract Vector2 GetVelocity(float maxSpeed);
    
    protected void RaiseEvent(EventHandler handler)
    {
        var raiseEvent = handler;
        raiseEvent?.Invoke(this, EventArgs.Empty);
    }
}