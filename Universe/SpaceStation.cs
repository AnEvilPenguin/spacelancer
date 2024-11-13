using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Spacelancer.Controllers;
using Spacelancer.Economy;

namespace Spacelancer.Universe;

public sealed class SpaceStation : IEntity
{
    public string Id { get; }
    public string Name { get; }
    private List<CommodityListing> _commodityListings { get; } = new List<CommodityListing>();
    

    public SpaceStation(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public void AddMarketListings(JToken listings)
    {
        foreach (var listing in listings)
        {
            var commodityId = listing.Value<string>("commodity");
            var transaction = listing.Value<string>("transaction");
            var price = listing.Value<int>("price");
            
            var commodity = Global.Economy.GetCommodity(commodityId);
            Enum.TryParse(transaction, out TransactionType transactionType);
            
            var newListing = new CommodityListing(commodity, transactionType, price);
            _commodityListings.Add(newListing);
        }
    }

    public List<CommodityListing> GetListings(TransactionType type) => 
        _commodityListings.Where(l => l.Type == type)
            .ToList();
    
    public List<CommodityListing> GetListings() =>
        new List<CommodityListing>(_commodityListings);

    public int GetListingPrice(Commodity commodity)
    {
        var listing = _commodityListings.FirstOrDefault(l => l.Commodity == commodity);
        
        if (listing != null)
            return listing.Price;
            
        return commodity.DefaultPrice;
    }
}