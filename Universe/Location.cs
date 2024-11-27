using Godot;

namespace Spacelancer.Universe;

public class Location
{
    public Vector2 Position { get; private set; }
    public int RotationDegrees { get; private set; }

    public Location(Vector2 position, int rotationDegrees)
    {
        Position = position;
        RotationDegrees = rotationDegrees;
    }
}