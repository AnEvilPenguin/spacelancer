using System;

namespace Spacelancer.Components.Equipment.Detection;

public class SensorDetectionEventArgs : EventArgs
{
    public ISensorDetectable Detection { get; init; }

    public SensorDetectionEventArgs(ISensorDetectable detection)
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