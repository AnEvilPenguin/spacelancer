using Godot;

namespace Spacelancer.Components.Navigation;

public interface INavigable
{
    public Marker2D GetNearestMarker(Vector2 position);

    public string GetName(Vector2 position);
}