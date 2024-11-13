using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Spacelancer.Controllers;
using Spacelancer.Util;

namespace Spacelancer.Universe;

public sealed class SolarSystem : IEntity
{
    public string Id { get; }
    public string Name { get; }
    
    private readonly List<JumpGate> _jumpGates = new();
    private readonly Dictionary<string, SpaceStation> _spaceStations = new();
    private readonly Dictionary<string, string> _spaceStationLookup = new();
    
    private JsonResource _contentLoader;
    
    private readonly JToken _configuration;

    public SolarSystem(string id, string name, JToken configuration)
    {
        Id = id;
        Name = name;
        _configuration = configuration;
    }
    
    public SpaceStation GetStation(string stationId) => 
        _spaceStations[stationId];
    
    public IEnumerable<SpaceStation> GetStations() => 
        _spaceStations.Values;
    
    public List<JumpGate> GetJumpGates() =>
        new (_jumpGates);

    public async Task LoadDependencies()
    {
        await Task.Factory.StartNew(() =>
        {
            _contentLoader = new JsonResource($"res://Configuration/Systems/{Id}");

            LoadStations();
            LoadJumpGates();
        });
    }

    private void LoadStations()
    {
        var stations = _contentLoader.GetTokenFromResource("Stations", "stations");

        foreach (var station in stations)
        {
            var id = station.Value<string>("id");
            var name = station.Value<string>("displayName");
            var location = Location.ConvertJTokenToLocation(station["location"]);
            
            var stationType = Enum.Parse<StationType>(station.Value<string?>("stationType") ?? "Generic");

            var newStation = new SpaceStation(id, name, location, stationType);
            
            _spaceStations.Add(id, newStation);
            _spaceStationLookup.Add(id, name);
        }
    }

    private void LoadJumpGates()
    {
        var gates = _contentLoader.GetTokenFromResource("JumpGates", "gates");

        foreach (var gate in gates)
        {
            var systemId = gate.Value<string>("system");
            var destination = Global.Universe.GetSystem(systemId);
            var location = Location.ConvertJTokenToLocation(gate["location"]);
            
            var newGate = new JumpGate($"{Id}_{systemId}", destination.Name, location);
            _jumpGates.Add(newGate);
        }
    }

}