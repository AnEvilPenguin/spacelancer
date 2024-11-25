using System;
using Godot;

namespace Spacelancer.Components.Navigation;

public class JumpEventArgs : EventArgs
{
    public Vector2 Target { get; init; }
    public JumpEventArgs(Vector2 targetPosition) =>
        Target = targetPosition;
        
}