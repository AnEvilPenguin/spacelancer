using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Serilog;
using Spacelancer.Economy;
using Spacelancer.Util;

namespace Spacelancer.Controllers;

public class EconomyController
{
    private static readonly JsonResource CommodityLoader = new("res://Configuration/");
    
    private readonly Dictionary<string, Commodity> _commodities = new();
    private readonly Dictionary<string, List<CommodityListing>> _commodityListings = new();
    
    public Commodity GetCommodity(string commodityId) =>
        _commodities[commodityId];

    public List<CommodityListing> GetListings(string stationId) =>
        _commodityListings[stationId];

    public void LoadEconomy()
    {
        LoadCommodities();
        LoadListings();
    }
        
    
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

    private void LoadListings()
    {
        _commodityListings.Clear();
        
        var stations = CommodityLoader.GetTokenFromResource("CommodityListings", "stations");

        foreach (var station in stations)
        {
            var id = station.Value<string>("station");
            
            var listings = station["listings"];
            
            var list = GetListings(listings);
            _commodityListings.Add(id, list);
        }
    }

    private List<CommodityListing> GetListings(JToken listings)
    {
        var output = new List<CommodityListing>();
        
        if (listings == null)
            return output;

        foreach (var listing in listings)
        {
            var commodityId = listing.Value<string>("commodity");
            var transaction = listing.Value<string>("transaction");
            var price = listing.Value<int>("price");
            
            var commodity = Global.Economy.GetCommodity(commodityId);
            Enum.TryParse(transaction, out TransactionType transactionType);
            
            var newListing = new CommodityListing(commodity, transactionType, price);
            output.Add(newListing);
        }
        
        return output;
    }
}