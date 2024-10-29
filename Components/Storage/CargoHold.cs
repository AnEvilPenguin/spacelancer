using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Spacelancer.Components.Commodities;

namespace Spacelancer.Components.Storage;

public class CargoHold
{
    public CommoditySize SizeClass { get; private set; }
    public int Capacity { get; private set; }

    private readonly Dictionary<CommodityType, Commodity> _cargoHold = new Dictionary<CommodityType, Commodity>();

    public CargoHold(CommoditySize sizeClass, int capacity)
    {
        SizeClass = sizeClass;
        Capacity = capacity;
    }
    
    public IEnumerable<CommodityType> GetCargoContents() => _cargoHold.Keys;

    public int GetUnusedCapacity() =>
        _cargoHold.Values.Aggregate(Capacity, (acc, cur) => acc - cur.GetVolume());

    public Commodity GetFromCargoHold(CommodityType type) =>
        _cargoHold.GetValueOrDefault(type);
    
    public void SetToCargoHold(CommodityType type, Commodity value)
    {
        if (value.Size > SizeClass)
        {
            Log.Error("{Cargo} of size {SizeClass} is too large to fit in cargo hold of {HoldSize}", value.Type, value.Size, SizeClass);
            throw new ArgumentOutOfRangeException($"{value.Type} of size {value.Size} is too large to fit in cargo hold of {SizeClass}");
        }

        var cargoVolume = value.GetVolume();
        var unusedCapacity = GetUnusedCapacity();
        
        if (cargoVolume > unusedCapacity)
        {
            Log.Error("{Cargo} of volume {CargoVolume} exceeds the free capacity of {UnusedCapacity}", value.Type, cargoVolume, unusedCapacity);
            throw new ArgumentOutOfRangeException(
                $"{value.Type} of volume {cargoVolume} exceeds the free capacity of {unusedCapacity}");
        }
        
        _cargoHold[type] = value;
    }
    
    public void RemoveFromCargoHold(CommodityType type)
    {
        if (_cargoHold.ContainsKey(type))
            _cargoHold.Remove(type);
    }
}