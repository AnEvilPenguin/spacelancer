using System.Collections.Generic;
using Spacelancer.Components.Commodities;

namespace Spacelancer.Components.Economy;

public class Economy
{
    
    private Dictionary<CommodityType, Commodity> _commodities;

    public Economy()
    {
        _commodities = new Dictionary<CommodityType, Commodity>()
        {
            {
                CommodityType.EnergyCell,
                new Commodity(
                    "Energy Cell", 
                    5,
                    "An energy cell is the most prolific thing in the universe.")
            },
            {
                CommodityType.Silicon,
                new Commodity(
                    "Silicon",
                    40,
                    "Silicon is used in the production of many electronic products.",
                    CommoditySize.Medium)
            },
            {
                CommodityType.Microcontroller,
                new Commodity(
                    "Microcontroller",
                    80,
                    "A small computer based on a single chip, used to build more complex electronic components")
            }
        };
    }

    public Commodity GetCommodity(CommodityType commodityType) =>
        _commodities[commodityType];

}