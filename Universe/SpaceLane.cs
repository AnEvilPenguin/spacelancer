namespace Spacelancer.Universe;

public class SpaceLane : IEntity
{
    public string Id { get; }
    public string Name => $"{Display1}_{Display2}";
    
    public string Display1 { get; }
    public string Display2 { get; }
    
    public Location Location1 { get; }
    public Location Location2 { get; }

    public SpaceLane(string id, string display1, string display2, Location location1, Location location2)
    {
        Id = id;
        Display1 = display1;
        Display2 = display2;
        Location1 = location1;
        Location2 = location2;
    }
}