using System;
using Spacelancer.Components.Navigation;

namespace Spacelancer.Scenes.UI.GameUI.Navigation;

public class AutopilotSelectedEventArgs : EventArgs
{
    public NavigationSoftwareType Button { get; init; }

    public AutopilotSelectedEventArgs(NavigationSoftwareType button)
    {
        Button = button;
    }
}
