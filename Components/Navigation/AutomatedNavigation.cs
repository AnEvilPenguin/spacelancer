using System;
using Godot;

namespace Spacelancer.Components.Navigation;

public abstract class AutomatedNavigation
{   
    public abstract event EventHandler Complete;
    public abstract event EventHandler Aborted;

    public abstract void DisruptTravel();
}