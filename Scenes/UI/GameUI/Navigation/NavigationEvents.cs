using System;

namespace Spacelancer.Scenes.UI.GameUI.Navigation;

public class AutopilotSelectedEventArgs : EventArgs
{
    public AutopilotButtonType Button { get; init; }

    public AutopilotSelectedEventArgs(AutopilotButtonType button)
    {
        Button = button;
    }
}
