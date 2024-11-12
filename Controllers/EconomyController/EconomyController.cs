using System.Collections.Generic;
using Spacelancer.Components.Commodities;
using Spacelancer.Components.Economy;

namespace Spacelancer.Controllers.EconomyController;

public class EconomyController
{
    // Long term we need to think about how we want to handle this.
    // Do we need a functioning long term economy, or can we just have something that only the player interacts with?
    // Initial plan: Just don't keep track of things. Let a player buy as much as they can carry, and sell as much as they have
    // Have a default price per commodity
    // Have specific overrides for specific stations

    private readonly Economy _economy = new Economy();

    public void LoadEconomy() =>
        _economy.LoadCommodities();

    public Commodity GetCommodity(string commodityId) =>
        _economy.GetCommodity(commodityId);
}