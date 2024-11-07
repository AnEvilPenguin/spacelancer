using Godot;
using System;
using Serilog;

public partial class StationMenu : PanelContainer
{
	private Station _selectedStation;

	private IconButton _commsButton;
	private IconButton _equipmentButton;
	private IconButton _shipyardButton;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var leaveButton = GetNode<IconButton>("%ExitButton");
		leaveButton.Pressed += OnLeaveButtonPressed;
		
		var tradeButton = GetNode<IconButton>("%TradeButton");
		tradeButton.Pressed += OnTradeButtonPressed;
		
		_commsButton = GetNode<IconButton>("%BarButton");
		_commsButton.Pressed += OnCommsButtonPressed;
		
		_equipmentButton = GetNode<IconButton>("%EquipmentButton");
		_shipyardButton = GetNode<IconButton>("%ShipyardButton");
		
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
		Visible = false;
		GetTree().Paused = false;
	}

	private void OnTradeButtonPressed()
	{
		Visible = false;
		var tradeMenu = Global.GameController.LoadScene<TradeMenu>("res://Scenes/UI/TradeMenu/trade_menu.tscn");
		tradeMenu.Visible = true;
		
		tradeMenu.LoadMenu(_selectedStation);
		tradeMenu.Closing += () =>
			Visible = true;
	}

	private void OnCommsButtonPressed()
	{
		Visible = false;
		var commsMenu = Global.GameController.LoadScene<CommsMenu>("res://Scenes/UI/CommsMenu/comms_menu.tscn");
		commsMenu.Visible = true;
		
		commsMenu.LoadNonPlayerCharacters(_selectedStation.GetNonPlayerCharacters());
		
		commsMenu.Closing += () =>
			Visible = true;
	}
}
