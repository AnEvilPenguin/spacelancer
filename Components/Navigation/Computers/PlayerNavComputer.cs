using Godot;
using Spacelancer.Components.Navigation.Software;
using Spacelancer.Controllers;
using Spacelancer.Scenes.Transitions;

namespace Spacelancer.Components.Navigation.Computers;

public sealed class PlayerNavComputer : AbstractNavigationComputer
{

    protected override INavigationSoftware CurrentSoftware
    {
        get => base.CurrentSoftware;
        set
        {
            base.CurrentSoftware = value;
            Global.UserInterface.ProcessNavigationSoftwareChange(value.Type);
        }
    }
    
    private Node2D _currentTarget;

    public PlayerNavComputer(INavigationSoftware backup) : base(backup)
    {
        CurrentSoftware = backup;

        Global.UserInterface.AutopilotButtonSelected += (sender, args) =>
        {
            if (args.Button == NavigationSoftwareType.Docking)
                ProcessAutopilotDocked();
            else if (args.Button == NavigationSoftwareType.Navigation)
                ProcessAutopilotNavigation();
            else
                ProcessAutopilotCancelled();
        };
    }
    
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
        
    // Do we need to figure out something better?
    // Can we send a softwareType or something?
    public INavigationSoftware GetCurrentSoftware() => CurrentSoftware;

    public void ProcessNewTarget(Node2D target) =>
        _currentTarget = target;

    public void ClearTarget() =>
        _currentTarget = null;
    
    // TODO manage queues of software (somehow)

    protected override void OnJump(object sender, JumpEventArgs e)
    {
        var destinationId = Global.Universe.GetSystemId(e.Destination);
        var destinationNode = Global.GameController.LoadSystem(destinationId);
        destinationNode.Visible = true;
                
        var exit = destinationNode.GetNode<JumpGate>($"{e.Origin}");
        
        SetAutomatedNavigation(new JumpExitNavigation(exit));
                
        Global.Player.GlobalPosition = exit.GlobalPosition;
        
        base.OnJump(sender, e);
    }

    private void ProcessAutopilotCancelled()
    {
        if (CurrentSoftware is not AutomatedNavigation automatedNavigation)
            return;
        
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
    
    // Queue of software? (would stack be better?)
    // method in to abort
    // signal for when complete
}