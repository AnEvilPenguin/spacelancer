using System;
using Godot;
using Spacelancer.Components.Navigation;

namespace Spacelancer.Scenes.Player;

public class PlayerNavComputer
{
    private readonly INavigationSoftware _backup;
    
    private INavigationSoftware _currentSoftware;

    public PlayerNavComputer(INavigationSoftware backup)
    {
        _backup = backup;
        _currentSoftware = backup;
    }

    public string Name => _currentSoftware.Name;
    public float GetRotation(float maxRotation) =>
        _currentSoftware.GetRotation(maxRotation);
    
    public Vector2 GetVelocity(float maxSpeed) =>
        _currentSoftware.GetVelocity(maxSpeed);
    
    public INavigationSoftware GetCurrentSoftware() => _currentSoftware;
    
    public void ResetNavSoftware() => _currentSoftware = _backup;

    public void SetAutomatedNavigation(AutomatedNavigation software)
    {
        _currentSoftware = software;

        software.Complete += OnAutopilotComplete;
        software.Aborted += OnAutopilotAborted;
    }

    private void OnAutopilotComplete(object sender, EventArgs e)
    {
        _currentSoftware = _backup;
    }

    private void OnAutopilotAborted(object sender, EventArgs e)
    {
        _currentSoftware = _backup;
    }
    
    // Queue of software?
    // method in to abort
    // signal for when complete
}