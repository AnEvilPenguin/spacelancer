using Spacelancer.Components.Commodities;

namespace Spacelancer.Components.Economy;

public class Economy
{
    public static Commodity EnergyCell => new Commodity(
        "Energy Cell", 
        5, 
        "An energy cell is the most prolific thing in the universe."
        );
    public static Commodity Silicon => new Commodity(
            "Silicon",
            40,
            "Silicon is used in the production of many electronic products.",
            CommoditySize.Medium
        );
    public static Commodity Microcontroller => new Commodity(
            "Microcontroller",
            80,
            "A small computer based on a single chip, used to build more complex electronic components"
        );
}