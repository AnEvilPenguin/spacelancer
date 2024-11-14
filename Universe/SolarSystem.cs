using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog;
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
    private readonly Dictionary<string, SpaceLane> _spaceLaneLookup = new();
    
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
    
    public List<SpaceLane> GetSpaceLanes() =>
        _spaceLaneLookup.Values.ToList();

    public async Task LoadDependencies()
    {
        await Task.Factory.StartNew(() =>
        {
            _contentLoader = new JsonResource($"res://Configuration/Systems/{Id}");

            LoadStations();
            LoadJumpGates();
            LoadSpaceLanes();
        });
    }

    private void LoadStations()
    {
        var stations = _contentLoader.GetTokenFromResource("Stations", "stations");
        
        if (stations == null)
        {
            Log.Warning("No Stations found for {systemId}:{systemName}", Id, Name);
            return;
        }

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
        
        if (gates == null)
        {
            Log.Warning("No gates found for {systemId}:{systemName}", Id, Name);
            return;
        }

        foreach (var gate in gates)
        {
            var systemId = gate.Value<string>("system");
            var destination = Global.Universe.GetSystem(systemId);
            var location = Location.ConvertJTokenToLocation(gate["location"]);
            
            var newGate = new JumpGate($"{Id}_{systemId}", destination.Name, location);
            _jumpGates.Add(newGate);
        }
    }

    private void LoadSpaceLanes()
    {
        var lanes = _contentLoader.GetTokenFromResource("SpaceLanes", "lanes");

        if (lanes == null)
        {
            Log.Debug("No lanes found for {systemId}:{systemName}", Id, Name);
            return;
        }

        foreach (var lane in lanes)
        {
            var laneId = lane.Value<string>("id");

            var display1 = lane["location1"]?.Value<string>("displayName");
            var display2 = lane["location2"]?.Value<string>("displayName");
            
            var location1 = Location.ConvertJTokenToLocation(lane["location1"]);
            var location2 = Location.ConvertJTokenToLocation(lane["location2"]);
            
            var newLane = new SpaceLane(laneId, LaneDisplayLookup(display1), LaneDisplayLookup(display2), location1, location2);
            _spaceLaneLookup.Add(laneId, newLane);
        }
    }

    private string LaneDisplayLookup(string lookup)
    {
        if (_spaceStations.TryGetValue(lookup, out var spaceStation))
        {
            return spaceStation.Name;
        }
        
        return lookup;
    }
}