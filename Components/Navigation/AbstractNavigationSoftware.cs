using Godot;

namespace Spacelancer.Components.Navigation;

public class AbstractNavigationSoftware
{
    // Is this class even needed?
    // Could mandate static factory in the interface instead? 
    
    protected readonly Node2D Parent;

    protected AbstractNavigationSoftware(Node2D parent)
    {
        Parent = parent;
    }
}