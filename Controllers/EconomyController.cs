using System;
using System.Collections.Generic;
using Serilog;
using Spacelancer.Economy;
using Spacelancer.Util;

namespace Spacelancer.Controllers;

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
        _commodities.Clear();
        var rawCommodities = CommodityLoader.GetTokenFromResource("Commodities", "commodities");

        foreach (var rawCommodity in rawCommodities)
        {
            var id = rawCommodity.Value<string>("id");
            var name = rawCommodity.Value<string>("displayName");
            var price = rawCommodity.Value<int>("defaultPrice");
            var description = rawCommodity.Value<string>("summary");
            var rawSize = rawCommodity.Value<string>("size");

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