using Godot;
using Spacelancer.Scenes.Player;

namespace Spacelancer.Components.Navigation;

public interface IDockable : INavigable
{
    public INavigationSoftware GetDockComputer(Player ship, INavigationSoftware nextSoftware);
}