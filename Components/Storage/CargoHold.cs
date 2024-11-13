using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Spacelancer.Economy;

namespace Spacelancer.Components.Storage;

public class CargoHold
{
    public CommoditySize SizeClass { get; private set; }
    public int Capacity { get; private set; }

    private readonly Dictionary<string, CommodityStack> _cargoHold = new Dictionary<string, CommodityStack>();

    public CargoHold(CommoditySize sizeClass, int capacity)
    {
        SizeClass = sizeClass;
        Capacity = capacity;
    }
    
    public IEnumerable<string> GetCargoContents() => _cargoHold.Keys;

    public int GetUnusedCapacity() =>
        _cargoHold.Values.Aggregate(Capacity, (acc, cur) => acc - cur.GetVolume());
    
    public int CheckCapacity(Commodity commodity) =>
        commodity.GetQuantityFromVolume(GetUnusedCapacity());

    public CommodityStack GetFromCargoHold(string type) =>
        _cargoHold.GetValueOrDefault(type);
    
    public void SetToCargoHold(string type, CommodityStack value)
    {
        var commodity = value.Commodity;
        
        if (commodity.Size > SizeClass)
        {
            Log.Error("{Cargo} of size {SizeClass} is too large to fit in cargo hold of {HoldSize}", commodity.Name, commodity.Size, SizeClass);
            throw new ArgumentOutOfRangeException($"{commodity.Name} of size {commodity.Size} is too large to fit in cargo hold of {SizeClass}");
        }

        var cargoVolume = value.GetVolume();
        var unusedCapacity = GetUnusedCapacity();
        
        if (cargoVolume > unusedCapacity)
        {
            Log.Error("{Cargo} of volume {CargoVolume} exceeds the free capacity of {UnusedCapacity}", commodity.Name, cargoVolume, unusedCapacity);
            throw new ArgumentOutOfRangeException(
                $"{commodity.Name} of volume {cargoVolume} exceeds the free capacity of {unusedCapacity}");
        }

        if (_cargoHold.TryGetValue(type, out var stack))
        {
            value = value.CombineStack(stack);
            _cargoHold.Remove(type);
        }
        
        _cargoHold[type] = value;
    }
    
    public void RemoveFromCargoHold(string type)
    {
        if (_cargoHold.ContainsKey(type))
            _cargoHold.Remove(type);
    }
}