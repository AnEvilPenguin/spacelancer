using System.Collections.Generic;

namespace Spacelancer.Controllers.Universe;

public class SolarSystem
{
    public string Name { get; }

    private readonly List<string> _jumpGateDestinations = new();

    public SolarSystem(string name)
    {
        Name = name;
    }
    
    public void AddJumpGateLink(IEnumerable<string> destinations) =>
        _jumpGateDestinations.AddRange(destinations);
}