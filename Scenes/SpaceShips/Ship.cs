using Godot;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Equipment.Storage;
using Spacelancer.Components.Navigation.Computers;

namespace Spacelancer.Scenes.SpaceShips;

public abstract partial class Ship : CharacterBody2D
{
    public CargoHold Hold { get; protected set; }
    public abstract AbstractNavigationComputer NavComputer { get; }
    
    protected const float MaxSpeed = 150.0f;
    protected abstract IdentificationFriendFoe IFF { get; set; }
    protected abstract Sensor Sensor { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        DebugNav();
		
        // TODO work out what a sensible max rotation is
        ProcessRotation();
        
        DebugRotation();
		
        ProcessVelocity();
		
        MoveAndSlide();
    }

    protected virtual void ProcessVelocity()
    {
        var velocity = NavComputer.GetVelocity(MaxSpeed, GlobalPosition, Velocity);

        var acceleration = velocity - Velocity;
        DebugAcceleration(acceleration);
		
        Velocity = velocity;
		
        DebugVelocity();
        DebugSpeed(Velocity.Length());
    }

    protected virtual void ProcessRotation()
    {
        Rotation = NavComputer.GetRotation(0, Rotation, Velocity);
    }

    protected virtual void DebugAcceleration(Vector2 acceleration)
    {}

    protected virtual void DebugVelocity()
    {}
    
    protected virtual void DebugNav() 
    {}
    
    protected virtual void DebugRotation() 
    {}
    
    protected virtual void DebugSpeed(float speed) 
    {}
}