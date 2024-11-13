using Godot;
using Newtonsoft.Json.Linq;

namespace Spacelancer.Universe;

public class Location
{
    
    public Vector2 Position { get; private set; }
    public int RotationDegrees { get; private set; }

    public static Location ConvertJTokenToLocation(JToken jToken)
    {
        return new Location
        {
            Position = new Vector2(jToken.Value<int>("X"), jToken.Value<int>("Y")),
            RotationDegrees = jToken.Value<int>("RotationDegrees")
        };
    }
}