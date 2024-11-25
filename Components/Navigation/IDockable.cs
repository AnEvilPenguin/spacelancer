using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Components.Navigation;

public interface IDockable : INavigable
{
    public AutomatedNavigation GetDockComputer();
}