using Godot;
using Spacelancer.Scenes.Player;

namespace Spacelancer.Components.Navigation;

public interface IDockable : INavigable
{
    public AutomatedNavigation GetDockComputer();
}