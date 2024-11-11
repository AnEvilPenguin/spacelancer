using Godot;
using System;
using Serilog;

public partial class StationMenu : PanelContainer
{
	private Station _selectedStation;

	private IconButton _playerButton;
	private IconButton _commsButton;
	private IconButton _equipmentButton;
	private IconButton _shipyardButton;
	
	private CommsMenu _commsMenu;
	private TradeMenu _tradeMenu;

	public override void _Ready()
	{
		AttachButtons();
		AttachMenus();
		
		// We can interact with this menu when the game is paused in the background.
		// We might not need this when we actually implement docking
		ProcessMode = ProcessModeEnum.Always;
	}

	public void ShowMenu(Station station)
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
		
		_tradeMenu.Visible = true;
		_tradeMenu.LoadMenu(_selectedStation);
	}

	private void OnCommsButtonPressed()
	{
		HideMenus();
		
		_commsMenu.Visible = true;
		_commsMenu.LoadNonPlayerCharacters(_selectedStation.GetNonPlayerCharacters());
	}

	private void AttachButtons()
	{
		var leaveButton = GetNode<IconButton>("%ExitButton");
		leaveButton.Pressed += OnLeaveButtonPressed;
		
		var tradeButton = GetNode<IconButton>("%TradeButton");
		tradeButton.Pressed += OnTradeButtonPressed;
		
		_playerButton = GetNode<IconButton>("%PlayerButton");
		_playerButton.Pressed += HideMenus;
		
		_commsButton = GetNode<IconButton>("%BarButton");
		_commsButton.Pressed += OnCommsButtonPressed;
		
		_equipmentButton = GetNode<IconButton>("%EquipmentButton");
		_shipyardButton = GetNode<IconButton>("%ShipyardButton");
	}

	private void AttachMenus()
	{
		_commsMenu = GetNode<CommsMenu>("%CommsMenu");
		_tradeMenu = GetNode<TradeMenu>("%TradeMenu");
		
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
