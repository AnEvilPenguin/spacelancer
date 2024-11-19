using System;

namespace Spacelancer.Components.Equipment.Detection;

public class SensorDetectionEventArgs : EventArgs
{
    public SensorDetection Detection { get; init; }

    public SensorDetectionEventArgs(SensorDetection detection)
    {
        Detection = detection;
    }
}

public class SensorLostEventArgs : EventArgs
{
    public ulong Id { get; init; }

    public SensorLostEventArgs(ulong id)
    {
        Id = id;
    }
}