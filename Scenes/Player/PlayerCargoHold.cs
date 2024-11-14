using Spacelancer.Components.Storage;
using Spacelancer.Economy;

namespace Spacelancer.Scenes.Player;

public partial class Player
{
    public CargoHold Hold = new CargoHold(CommoditySize.Medium, 100);

    public int Credits = 500;
}