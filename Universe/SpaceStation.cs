using System;
using System.Collections.Generic;
using System.Linq;
using Spacelancer.Controllers;
using Spacelancer.Economy;

namespace Spacelancer.Universe;

public sealed class SpaceStation : IEntity
{
    public string Id { get; }
    public string Name { get; }
    public Location Location { get; }
    public StationType Type { get; }

    public SpaceStation(string id, string name, Location location, StationType type)
    {
        Id = id;
        Name = name;
        Location = location;
        Type = type;
    }

    public List<CommodityListing> GetListings(TransactionType type) =>
        Global.Economy.
            GetListings(Id)
            .Where(l => l.Type == type)
            .ToList();
    
    public List<CommodityListing> GetListings() =>
        Global.Economy.
            GetListings(Id);

    public int GetListingPrice(Commodity commodity)
    {
        var listing = GetListings().FirstOrDefault(l => l.Commodity == commodity);
        
        if (listing != null)
            return listing.Price;
            
        return commodity.DefaultPrice;
    }
}