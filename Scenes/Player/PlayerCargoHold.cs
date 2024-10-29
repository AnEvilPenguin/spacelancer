using Spacelancer.Components.Commodities;
using Spacelancer.Components.Storage;

public partial class Player
{
    public CargoHold Hold = new CargoHold(CommoditySize.Medium, 100);

    public int Credits = 500;
}