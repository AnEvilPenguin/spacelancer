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

    public Vector2 GetVelocity(float maxSpeed)
    {
        // TODO check for user input around F1, F2, F3
        // Take appropriate action depending on current type e.g. force abort.
        
        return _currentSoftware.GetVelocity(maxSpeed);
    }
        
    
    public INavigationSoftware GetCurrentSoftware() => _currentSoftware;

    public void ProcessNewTarget(Node2D target)
    {
        // TODO if dockable
        // TODO if navigable
        // Do we need to consider what we are already doing?
    }

    public void ClearTarget()
    {
        // TODO deactivate whatever we need to deactivate unless it's related to software we're using
        // e.g. deactivate autopilot and leave dock active if we have dock software
    }
    
    // TODO on new target check what buttons can be enabled
    // TODO on target lost check what buttons can be disabled
    // TODO manage what active button should be
    // TODO deal with requesting software from target
    // TODO manage queues of software (somehow)
    // TODO computer type instead of button type. Just use that
    // TODO add type to interface
    
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