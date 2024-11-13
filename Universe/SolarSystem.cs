using System.Collections.Generic;
using Spacelancer.Util;

namespace Spacelancer.Universe;

public sealed class SolarSystem : IEntity
{
    public string Id { get; }
    public string Name { get; }
    
    private readonly List<string> _jumpGateDestinations = new();
    private readonly Dictionary<string, SpaceStation> _spaceStations = new();
    private readonly Dictionary<string, string> _spaceStationLookup = new();

    public SolarSystem(string id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public SpaceStation GetStation(string stationId) => 
        _spaceStations[stationId];
    
    public void AddJumpGateLink(IEnumerable<string> destinations) =>
        _jumpGateDestinations.AddRange(destinations);

    public void LoadStations()
    {
        var contentLoader = new JsonResource($"res://Configuration/Systems/{Id}");
        var stations = contentLoader.GetTokenFromResource("Stations", "stations");

        foreach (var station in stations)
        {
            var id = station.Value<string>("id");
            var name = station.Value<string>("displayName");

            var newStation = new SpaceStation(id, name);
            
            _spaceStations.Add(id, newStation);
            _spaceStationLookup.Add(id, name);
            
            var listings = station["listings"];
            if (listings != null)
                newStation.AddMarketListings(listings);
        }
    }

}