using System;
using System.Collections.Generic;
using Serilog;
using Spacelancer.Components.Economy.Commodities;
using Spacelancer.Util;

namespace Spacelancer.Controllers.EconomyController;

public class EconomyController
{
    private static readonly JsonResource CommodityLoader = new("res://Configuration/");
    
    private readonly Dictionary<string, Commodity> _commodities = new();
    
    public Commodity GetCommodity(string commodityId) =>
        _commodities[commodityId];

    public void LoadEconomy() =>
        LoadCommodities();
    
    private void LoadCommodities()
    {
        var configuration = CommodityLoader.LoadFromResource("Commodities");

        if (!configuration.TryGetValue("commodities", out var rawCommodities))
        {
            Log.Error("Commodities configuration file not loaded correctly {rawData}", configuration);
            throw new InvalidOperationException("Commodities configuration file not loaded correctly");
        }

        foreach (var rawCommodity in rawCommodities)
        {
            var id = rawCommodity.Value<String>("id");
            var name = rawCommodity.Value<String>("displayName");
            var price = rawCommodity.Value<int>("defaultPrice");
            var description = rawCommodity.Value<String>("summary");
            var rawSize = rawCommodity.Value<String>("size");

            if (!Enum.TryParse(rawSize, out CommoditySize size))
            {
                Log.Error("Commodity size for {id} is not valid {rawSize}", id,rawSize);
                break;
            }
            
            var commodity = new Commodity(name, price, description, size);
            
            _commodities.Add(id, commodity);
        }
    }
}