﻿using Godot;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.Equipment.Storage;
using Spacelancer.Components.Navigation;
using Spacelancer.Economy;

namespace Spacelancer.Scenes.Player;

public partial class Player
{
    public CargoHold Hold;
    
    public INavigationSoftware NavComputer;

    public int Credits = 500;

    private Sensor _sensor;

    private IdentificationFriendFoe _iff;

    private void SetDefaultEquipment()
    {
        _sensor = new Sensor(10_000f);
        AddChild(_sensor);

        var detection = new SensorDetection("Player", "Temp", SensorDetectionType.Ship, this);
        _iff = new IdentificationFriendFoe(this, detection);
        
        Hold = new CargoHold(CommoditySize.Medium, 100);
        NavComputer = new PlayerNavigation(this);
    }
}