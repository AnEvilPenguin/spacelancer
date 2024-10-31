﻿using System.ComponentModel;
using Spacelancer.Components.Commodities;

namespace Spacelancer.Controllers.EconomyController;

public class EconomyController
{
    // Long term we need to think about how we want to handle this.
    // Do we need a functioning long term economy, or can we just have something that only the player interacts with?
    // Initial plan: Just don't keep track of things. Let a player buy as much as they can carry, and sell as much as they have
    // Have a default price per commodity
    // Have specific overrides for specific stations

    public static int GetDefaultPrice(CommodityType type) =>
        type switch
        {
            CommodityType.EnergyCell => 8,
            _ => throw new InvalidEnumArgumentException($"Invalid commodity type {type}")
        };
}