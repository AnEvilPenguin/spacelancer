using System.Collections.Generic;
using Godot;
using Serilog;

namespace Spacelancer.Components.Equipment.Detection;

// TODO consider an IdentificationFriendFoe class that handles things like affiliation?

public partial class Sensor : Node2D
{
    private readonly Dictionary<ulong, SensorDetection> _detectedObjects = new();

    public Sensor(float radius)
    {
        var area2D = new Area2D();
        var collisionShape2D = new CollisionShape2D();
        var circleShape = new CircleShape2D();
        
        circleShape.Radius = radius;
        collisionShape2D.Shape = circleShape;
        
        area2D.AddChild(collisionShape2D);
        AddChild(area2D);
        
        ConfigureDetectionArea(area2D);
    }

    public Sensor() {}

    private void ConfigureDetectionArea(Area2D area)
    {
        area.AreaEntered += OnBodyEntered;
        area.AreaExited += OnBodyExited;
    }

    private void OnBodyEntered(Area2D body)
    {
        if (body is not ISensorDetectable detectableBody) 
            return;
        
        if (body.GetInstanceId() == GetParent().GetInstanceId())
            return;
        
        _detectedObjects.Add(detectableBody.GetInstanceId(), detectableBody.Detect());
    }

    private void OnBodyExited(Area2D body)
    {
        if (body is not ISensorDetectable detectableBody) 
            return;
        
        if (body.GetInstanceId() == GetParent().GetInstanceId())
            return;

        var id = detectableBody.GetInstanceId();

        if (!_detectedObjects.ContainsKey(id))
        {
            Log.Error("Sensor detected object {Id} not found in lookup", id);
            return;
        }
        
        _detectedObjects.Remove(id);
    }
}