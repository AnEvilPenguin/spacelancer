using System;
using Godot;
using Spacelancer.Components.Navigation;
using Spacelancer.Controllers;

namespace Spacelancer.Scenes.SpaceShips;

public class PlayerNavComputer
{
    private readonly INavigationSoftware _backup;
    
    private INavigationSoftware CurrentSoftware
    {
        get => _currentSoftware;
        set
        {
            _currentSoftware = value;
            Global.UserInterface.ProcessNavigationSoftwareChange(_currentSoftware.Type);
        }
    }

    private INavigationSoftware _currentSoftware;
    
    private Node2D _currentTarget;

    public PlayerNavComputer(INavigationSoftware backup)
    {
        _backup = backup;
        CurrentSoftware = backup;
    }

    public string Name => CurrentSoftware.Name;
    
    public float GetRotation(float maxRotation, float currentAngle, Vector2 currentVelocity) =>
        CurrentSoftware.GetRotation(maxRotation, currentAngle, currentVelocity);

    public Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity) =>
        CurrentSoftware.GetVelocity(maxSpeed, currentPosition, currentVelocity);
    
    public void CheckForNavigationInstructions()
    {
        // We're not going to entertain someone pressing all buttons at once.
        if (Input.IsActionJustPressed("AutoPilotCancel"))
            ProcessAutopilotCancelled();
        else if (Input.IsActionJustPressed("AutoPilotDestination"))
            ProcessAutopilotNavigation();
        else if (Input.IsActionJustPressed("AutoPilotDock"))
            ProcessAutopilotDocked();
    }
        
    
    public INavigationSoftware GetCurrentSoftware() => CurrentSoftware;

    public void ProcessNewTarget(Node2D target) =>
        _currentTarget = target;

    public void ClearTarget() =>
        _currentTarget = null;
    
    // TODO on new target check what buttons can be enabled
    // TODO on target lost check what buttons can be disabled
    // TODO manage what active button should be
    // TODO deal with requesting software from target
    // TODO manage queues of software (somehow)
    // TODO computer type instead of button type. Just use that
    // TODO add type to interface
    
    public void ResetNavSoftware() => CurrentSoftware = _backup;

    public void SetAutomatedNavigation(AutomatedNavigation software)
    {
        CurrentSoftware = software;

        software.Complete += OnAutopilotComplete;
        software.Aborted += OnAutopilotAborted;
    }

    private void ProcessAutopilotCancelled()
    {
        if (CurrentSoftware is AutomatedNavigation automatedNavigation)
            automatedNavigation.DisruptTravel();
    }

    private void ProcessAutopilotNavigation()
    {
        if (_currentTarget is not INavigable target)
            return;

        var autoPilot = new SystemAutoNavigation(target);
        SetAutomatedNavigation(autoPilot);
    }

    private void ProcessAutopilotDocked()
    {
        if (_currentTarget is not IDockable target)
            return;

        // FIXME if not within a certain range call Autopilot instead?
        var autoPilot = target.GetDockComputer();
        SetAutomatedNavigation(autoPilot);
    }

    private void OnAutopilotComplete(object sender, EventArgs e)
    {
        CurrentSoftware = _backup;
    }

    private void OnAutopilotAborted(object sender, EventArgs e)
    {
        CurrentSoftware = _backup;
    }
    
    // Queue of software?
    // method in to abort
    // signal for when complete
}