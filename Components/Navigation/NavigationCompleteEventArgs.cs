using System;
using Spacelancer.Scenes.Stations;

namespace Spacelancer.Components.Navigation;

public enum NavigationCompleteType
{
    Aborted,
    Completed,
    Jumped,
    Docked
}

public class JumpNavigationCompleteEventArgs : NavigationCompleteEventArgs
{
    public string Origin { get; }
    public string Destination { get; }

    public JumpNavigationCompleteEventArgs(string origin, string destination) : base(NavigationCompleteType.Jumped)
    {
        Origin = origin;
        Destination = destination;
    }
}

public class DockingNavigationCompleteEventArgs : NavigationCompleteEventArgs
{
    public Station Destination { get; }

    public DockingNavigationCompleteEventArgs(Station destination) : base(NavigationCompleteType.Docked)
    {
        Destination = destination;
    }
}

public class AbortedNavigationCompleteEventArgs : NavigationCompleteEventArgs
{
    public AbortedNavigationCompleteEventArgs() : base(NavigationCompleteType.Aborted)
    {}
}

public class GenericNavigationCompleteEventArgs : NavigationCompleteEventArgs
{
    public GenericNavigationCompleteEventArgs() : base(NavigationCompleteType.Completed)
    {}
}

public abstract class NavigationCompleteEventArgs : EventArgs
{
    public NavigationCompleteType Type { get; }
    
    public NavigationCompleteEventArgs(NavigationCompleteType type)
    {
        Type = type;
    }
}