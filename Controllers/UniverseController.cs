using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Serilog;
using Spacelancer.Scenes.SolarSystems;
using Spacelancer.Scenes.Stations;
using Spacelancer.Universe;
using Spacelancer.Util;

namespace Spacelancer.Controllers;

public class UniverseController
{
    private readonly Dictionary<string, SpaceStation> _spaceStations = new();
    private readonly Dictionary<string, string> _systemNames = new();
    private readonly Dictionary<string, PackedScene> _systemScenes = new();
    
    public string GetSystemId(string systemName) =>
        _systemNames[systemName];

    public BaseSystem GetSystemInstance(string systemId)
    {
        if (!_systemScenes.TryGetValue(systemId, out var system))
        {
            Log.Warning($"System ID {systemId} not found");
            return null;
        }
        
        return system.Instantiate<BaseSystem>();
    }
    
    public SpaceStation GetSpaceStation(string stationId) => 
        _spaceStations[stationId];

    public void LoadSystemScenes()
    {
        var path = "res://Scenes/SolarSystems/";
        using var dir = DirAccess.Open(path);
        if (dir == null)
        {
            Log.Error("Failed to load system scenes");
            throw new Exception("Failed to load system scenes");
        }
        
        dir.ListDirBegin();
        string fileName = dir.GetNext();
        
        while (!String.IsNullOrWhiteSpace(fileName))
        {
            if (dir.CurrentIsDir())
            {
                fileName = dir.GetNext();
                continue;
            }
            
            if (fileName == "base_system.tscn" || fileName == "BaseSystem.cs")
            {
                fileName = dir.GetNext();
                continue;
            }
            
            Log.Debug("Trying to load system scene {Name}", fileName);
            
            var scene = GD.Load<PackedScene>($"{path}{fileName}");
            var system = scene.Instantiate<BaseSystem>();
            
            LoadStations(system);
            
            _systemScenes.Add(system.Id, scene);
            _systemNames.Add(system.Name, system.Id);
            
            system.QueueFree();
            
            fileName = dir.GetNext();
        }
    }

    private void LoadStations(BaseSystem system)
    {
        var stations = system.GetChildren().OfType<Station>();

        foreach (var station in stations)
        {
            var location = new Location(station.Position, (int)station.RotationDegrees);
            
            var stationConfig = new SpaceStation(station.Id, station.Name, location);
            _spaceStations.Add(station.Id, stationConfig);
        }
    }
}