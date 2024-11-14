using Godot;
using Spacelancer.Controllers;

namespace Spacelancer.Scenes.UI.StationMenu;

public partial class StationMenu : PanelContainer
{
	private Stations.Station _selectedStation;

	private IconButton.IconButton _playerButton;
	private IconButton.IconButton _commsButton;
	private IconButton.IconButton _equipmentButton;
	private IconButton.IconButton _shipyardButton;
	
	private CommsMenu.CommsMenu _commsMenu;
	private TradeMenu.TradeMenu _tradeMenu;

	public override void _Ready()
	{
		AttachButtons();
		AttachMenus();
		
		// We can interact with this menu when the game is paused in the background.
		// We might not need this when we actually implement docking
		ProcessMode = ProcessModeEnum.Always;
	}

	public void ShowMenu(Stations.Station station)
	{
		Visible = true;
		GetTree().Paused = true;
		
		_selectedStation = station;

		_commsButton.Visible = station.HasNpc();
		_equipmentButton.Visible = station.HasEquipment();
		_shipyardButton.Visible = station.HasShipyard();
	}

	private void OnLeaveButtonPressed()
	{
		HideMenus();
		
		Visible = false;
		GetTree().Paused = false;
	}

	private void OnTradeButtonPressed()
	{
		HideMenus();
		
		var spaceStation = Global.Universe.GetSpaceStation(_selectedStation.Id);
		
		_tradeMenu.Visible = true;
		_tradeMenu.LoadMenu(spaceStation);
	}

	private void OnCommsButtonPressed()
	{
		HideMenus();
		
		_commsMenu.Visible = true;

		var npcs = _selectedStation.GetNonPlayerCharacters();

		npcs.ForEach(npc => npc.LoadDialog(_selectedStation.Id));
		
		_commsMenu.LoadNonPlayerCharacters(npcs);
	}

	private void AttachButtons()
	{
		var leaveButton = GetNode<IconButton.IconButton>("%ExitButton");
		leaveButton.Pressed += OnLeaveButtonPressed;
		
		var tradeButton = GetNode<IconButton.IconButton>("%TradeButton");
		tradeButton.Pressed += OnTradeButtonPressed;
		
		_playerButton = GetNode<IconButton.IconButton>("%PlayerButton");
		_playerButton.Pressed += HideMenus;
		
		_commsButton = GetNode<IconButton.IconButton>("%BarButton");
		_commsButton.Pressed += OnCommsButtonPressed;
		
		_equipmentButton = GetNode<IconButton.IconButton>("%EquipmentButton");
		_shipyardButton = GetNode<IconButton.IconButton>("%ShipyardButton");
	}

	private void AttachMenus()
	{
		_commsMenu = GetNode<CommsMenu.CommsMenu>("%CommsMenu");
		_tradeMenu = GetNode<TradeMenu.TradeMenu>("%TradeMenu");
		
		HideMenus();
	}

	private void HideMenus()
	{
		_commsMenu.Visible = false;
		_commsMenu.ClearChat();
		
		_tradeMenu.Visible = false;
		_tradeMenu.ClearMenu();
	}
}