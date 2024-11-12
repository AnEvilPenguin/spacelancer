using System.Collections.Generic;
using Spacelancer.Util;
using Spacelancer.Controllers.Universe;

namespace Spacelancer.Controllers;

public class UniverseController
{
    private static readonly JsonResource ConfigurationLoader = new("res://Configuration/");
    
    private readonly Dictionary<string, SolarSystem> _systems = new();
    private readonly Dictionary<string, string> _systemNames = new();
    
    public SolarSystem GetSystem(string systemId) => 
        _systems[systemId];

    public void LoadUniverse()
    {
        LoadSystems();
    }

    private void LoadSystems()
    {
        var systems = ConfigurationLoader.GetTokenFromResource("Systems", "systems");

        foreach (var system in systems)
        {
            var id = system.Value<string>("id");
            var name = system.Value<string>("displayName");

            var newSystem = new SolarSystem(name);

            var jumpGates = system["jumpGates"];
            if (jumpGates != null)
                newSystem.AddJumpGateLink(jumpGates.ToObject<IEnumerable<string>>());
            
            _systems.Add(id, newSystem);
            _systemNames.Add(name, id);
        }
    }
}