using System;
using Godot;

namespace Spacelancer.Components.Navigation;

public class JumpEventArgs : EventArgs
{
    public string Destination { get; }
    public string Origin { get; }

    public JumpEventArgs(string destination, string origin)
    {
        Destination = destination;
        Origin = origin;
    }
}