using System.Collections.Generic;
using Godot;
using Serilog;

namespace Spacelancer.Components.Equipment.Detection;

// TODO consider an IdentificationFriendFoe class that handles things like affiliation?

public partial class Sensor : Node2D
{
    public event SensorLostEventHandler SensorLost;
    public delegate void SensorLostEventHandler(object sender, SensorLostEventArgs e);
    
    public event SensorDetectionEventHandler SensorDetection; 
    public delegate void SensorDetectionEventHandler(object sender, SensorDetectionEventArgs e);
    
    private readonly Dictionary<ulong, ISensorDetectable> _detectedObjects = new();
    
    private ISensorDetectable _currentTarget;
    
    private readonly Node2D _parent;
    
    public Sensor(float radius, Node2D parent)
    {
        _parent = parent;
        
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

    public IEnumerable<ISensorDetectable> GetDetections() =>
        _detectedObjects.Values;

    public void LockTarget(ulong id)
    {
        if (_detectedObjects.TryGetValue(id, out var o))
            _currentTarget = o;
    }
    
    public void ISensorDetectable (ISensorDetectable target) => 
        _currentTarget = target;
        
    public ISensorDetectable GetLockedTarget() =>
        _currentTarget;
    
    public void ClearLockedTarget() =>
        _currentTarget = null;

    private void ConfigureDetectionArea(Area2D area)
    {
        area.AreaEntered += OnBodyEntered;
        area.AreaExited += OnBodyExited;
    }

    private void OnBodyEntered(Area2D body)
    {
        // Ignore Areas not associated with IFF
        if (body is not IdentificationFriendFoe detectableBody) 
            return;
        
        var detection = detectableBody.Detect();
        var id = detection.GetInstanceId();
        
        // Ignore own IFF
        if (id == GetParent().GetInstanceId())
            return;
        
        _detectedObjects.Add(id, detection);
        
        var raiseEvent = SensorDetection;

        if (raiseEvent != null)
            raiseEvent(this, new SensorDetectionEventArgs(detection));
        
    }

    private void OnBodyExited(Area2D body)
    {
        if (body is not IdentificationFriendFoe detectableBody) 
            return;
        
        var detection = detectableBody.Detect();
        var id = detection.GetInstanceId();
        
        if (id == GetParent().GetInstanceId())
            return;

        if (!_detectedObjects.ContainsKey(id))
        {
            Log.Error("Sensor detected object {Id} not found in lookup", id);
            return;
        }
        
        if (_currentTarget != null && _currentTarget.GetInstanceId() == id)
            ClearLockedTarget();
        
        _detectedObjects.Remove(id);
        
        var raiseEvent = SensorLost;

        if (raiseEvent != null)
            raiseEvent(this, new SensorLostEventArgs(id));
    }
}