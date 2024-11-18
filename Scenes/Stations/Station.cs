using System;
using System.Collections.Generic;
using Godot;
using Serilog;
using Spacelancer.Components.Equipment.Detection;
using Spacelancer.Components.NPCs;
using Spacelancer.Economy;
using Spacelancer.Scenes.UI.StationMenu;
using Spacelancer.Universe;
using Spacelancer.Util;

namespace Spacelancer.Scenes.Stations;

public partial class Station : Node2D
{
	[Export]
	public string Id;
	
	// We may need to consider making this docking range if we make a map and remote comms or something
	private bool _playerInCommsRange = false;
	private UI.StationMenu.StationMenu _menu;
	
	private readonly List<Tuple<Commodity, int>> _commoditiesForSale = new List<Tuple<Commodity, int>>();
	private readonly Dictionary<string, int> _commodityBuyPriceOverride = new Dictionary<string, int>();
	
	private readonly List<NonPlayerCharacter> _nonPlayerCharacters = new List<NonPlayerCharacter>();
	
	private IdentificationFriendFoe _iff;
	
	public override void _Ready()
	{
		var stationBorder = GetNode<Area2D>("Area2D");
		
		stationBorder.BodyEntered += OnStationAreaEntered;
		stationBorder.BodyExited += OnStationAreaExited;
		
		var detection = new SensorDetection(Name, "TODO", SensorDetectionType.Station, this);
		_iff = new IdentificationFriendFoe(this, detection);
		
		LoadNpcs();
	}

	public override void _Process(double delta)
	{
		if (!_playerInCommsRange)
			return;

		if (Input.IsKeyPressed(Key.C) && (_menu == null || !_menu.Visible))
		{
			Log.Debug("Comms initiated with {StationName}", Name);
			_menu = Controllers.Global.GameController.LoadScene<UI.StationMenu.StationMenu>("res://Scenes/UI/StationMenu/station_menu.tscn");
			_menu.ShowMenu(this);
		}
	}

	public void LoadNpcs()
	{
		_nonPlayerCharacters.Clear();

		var npcReader = new JsonResource($"res://Configuration/Stations/{Id}");
		var npcs = npcReader.GetTokenFromResource("Npcs", "characters");
		
		if (npcs == null)
		{
			Log.Warning("No NPCs found for {systemId}:{systemName}", Id, Name);
			return;
		}
		
		foreach (var npcConfig in npcs)
		{
			var id = npcConfig.Value<string>("id");
			var name = npcConfig.Value<string>("displayName");
			var summary = npcConfig.Value<string>("summary");
			var affiliation = npcConfig.Value<string>("affiliation");
			
			var newNpc = new NonPlayerCharacter(id, name, summary, affiliation);
			_nonPlayerCharacters.Add(newNpc);
		}
	}
	
	public List<NonPlayerCharacter> GetNonPlayerCharacters() => 
		new List<NonPlayerCharacter>(_nonPlayerCharacters);
	
	public bool HasNpc() =>
		_nonPlayerCharacters.Count > 0;
	
	public bool HasShipyard() => false;
	
	public bool HasEquipment() => false;

	public void AddCommodityForSale(Commodity commodity) =>
		AddCommodityForSale(commodity, commodity.DefaultPrice);

	public void AddCommodityForSale(Commodity commodity, int price)
	{
		_commoditiesForSale.Add(new Tuple<Commodity, int>(commodity, price));
		AddCommodityToBuy(commodity, price);
	}

	public void AddCommodityToBuy(Commodity commodity, int price) =>
		_commodityBuyPriceOverride.Add(commodity.Name, price);

	public List<Tuple<Commodity, int>> GetCommodityForSale() => 
		new List<Tuple<Commodity, int>>(_commoditiesForSale);

	public int GetCommodityToBuyPrice(Commodity commodity)
	{
		if (_commodityBuyPriceOverride.TryGetValue(commodity.Name, out int price))
			return price;
		
		return commodity.DefaultPrice;
	}

	private void OnStationAreaEntered(Node2D body)
	{
		if (body is Player.Player)
		{
			_playerInCommsRange = true;
			Log.Debug("Player in comms range of {StationName}", Name);
			
			var label = Controllers.Global.GameController.TempStationLabel;
			label.Text = $"Press C to talk to {Name}";
			label.Visible = true;
		}
	}

	private void OnStationAreaExited(Node2D body)
	{
		if (body is Player.Player)
		{
			_playerInCommsRange = false;
			Log.Debug("Player left comms range of {StationName}", Name);
			
			var label = Controllers.Global.GameController.TempStationLabel;
			label.Visible = false;
		}
	}
}