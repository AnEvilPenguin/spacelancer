using System;
using Godot;
using Serilog;
using Spacelancer.Scenes.Transitions;

namespace Spacelancer.Components.Navigation.Software;

public class JumpExitNavigation : AutomatedNavigation
{
    private enum JumpState
    {
        Exiting,
        Complete
    }
    
    public override event EventHandler Complete;
    public override event EventHandler Aborted;

    private readonly Vector2 _destination;
    private JumpState _state = JumpState.Exiting;
    
    public override string Name => $"JumpExit - {_state}";
    public override NavigationSoftwareType Type => NavigationSoftwareType.Docking;

    public JumpExitNavigation(JumpGate exit)
    {
        _destination = exit.GetExitMarker();;
    }
    
    public override float GetRotation(float maxRotation, float currentRotation, Vector2 currentVelocity) =>
        currentVelocity.Angle();

    public override Vector2 GetVelocity(float maxSpeed, Vector2 currentPosition, Vector2 currentVelocity) =>
        _state switch
        {
            JumpState.Exiting => ProcessExitingVector(currentPosition),
            JumpState.Complete => ProcessCompleteVector(),
            _ => Vector2.Zero
        };
    
    private Vector2 ProcessExitingVector(Vector2 currentPosition)
    {
        var proposed = _destination - currentPosition;
        
        if (proposed.Length() < 5)
        {
            SetState(JumpState.Complete);
            return Vector2.Zero;
        }
        
        return proposed.LimitLength(50);
    }

    private Vector2 ProcessCompleteVector()
    {
        RaiseEvent(Complete);
        return Vector2.Zero;
    }
    
    // TODO Can we make this generic?
    private void SetState(JumpState newState)
    {
        Log.Debug("{Name} state change from {OldState} to {NewState}", Name, _state, newState);
        _state = newState;
    }
    
    // Not a thing in this case
    public override void DisruptTravel()
    {
    }
}