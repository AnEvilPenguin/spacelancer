using System;
using System.Collections.Generic;
using Godot;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Components.Navigation.Computers;

public abstract class AbstractNavigationComputer : INavigationSoftware
{
    public event EventHandler Changed;
    public event EventHandler Complete;
    public event EventHandler Disrupted;
    // public abstract event EventHandler Docking;
    public event EventHandler<JumpEventArgs> Jumping;
    
    public string Name => _currentSoftware.Name;
    public NavigationSoftwareType Type => _currentSoftware.Type;
    public float GetRotation(float maxRotation, float currentAngle, Vector2 currentVelocity) =>
        _currentSoftware.GetRotation(maxRotation, currentAngle, currentVelocity);

    public Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity) =>
        _currentSoftware.GetVelocity(maxSpeed, currentPosition, currentVelocity);
    
    private readonly INavigationSoftware _backup;
    private INavigationSoftware _currentSoftware;
    
    private Stack<AutomatedNavigation> _navigationStack;

    protected virtual INavigationSoftware CurrentSoftware
    {
        get => _currentSoftware;
        set
        {
            _currentSoftware = value;
            RaiseEvent(Changed);
        }
    }

    protected AbstractNavigationComputer(INavigationSoftware backup)
    {
        _backup = backup;
        _currentSoftware = backup;
    }
    
    // TODO can we get rid of this?
    // Maybe replace with a type?
    public INavigationSoftware GetCurrentSoftware() => _currentSoftware;
    
    public void SetAutomatedNavigation(AutomatedNavigation software)
    {
        CurrentSoftware = software;
        
        if (software is JumpEntranceNavigation jumpNavigation)
        {
            // TODO Think about this more. Can we hoof event generation off to abstract
            // Can we hoof more things in general off to the abstract?
            jumpNavigation.Jumping += OnJump;
        }
        
        // TODO StationNavigation

        software.Complete += OnAutopilotComplete;
        software.Aborted += OnAutopilotAborted;
    }

    protected virtual void OnJump(object sender, JumpEventArgs e)
    {
        var raiseEvent = Jumping;
        raiseEvent?.Invoke(this, e);
    }
    
    public void SetNavigationStack(Stack<AutomatedNavigation> stack) => 
        _navigationStack = stack;
    
    private void OnAutopilotComplete(object sender, EventArgs e)
    {
        if (_navigationStack == null || _navigationStack.Count == 0)
        {
            RaiseEvent(Complete);
            CurrentSoftware = _backup;
            return;
        }
        
        SetAutomatedNavigation(_navigationStack.Pop());
    }

    private void OnAutopilotAborted(object sender, EventArgs e)
    {
        RaiseEvent(Disrupted);
        CurrentSoftware = _backup;
    }
    
    protected void RaiseEvent(EventHandler handler)
    {
        var raiseEvent = handler;
        raiseEvent?.Invoke(this, EventArgs.Empty);
    }
}