using Godot;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Components.Navigation;

public interface IDockable : INavigable
{
    public Vector2 GlobalPosition { get; }
    public string Id { get; }
    public AutomatedNavigation GetDockComputer();
}