namespace Spacelancer.Universe;

public class JumpGate : IEntity
{
    public string Id { get; }
    public string Name { get; }
    public Location Location { get; }

    public JumpGate(string id, string name, Location location)
    {
        Id = id;
        Name = name;
        Location = location;
    }
}