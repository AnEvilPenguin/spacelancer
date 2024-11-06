using Godot;
using System;
using Serilog;

public partial class StationMenu : CenterContainer
{
	private Station _selectedStation;

	private Button _commsButton;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var leaveButton = GetNode<Button>("%Leave");
		leaveButton.Pressed += OnLeaveButtonPressed;

		var tradeButton = GetNode<Button>("%Trade");
		tradeButton.Pressed += OnTradeButtonPressed;
		
		_commsButton = GetNode<Button>("%Comms");
		_commsButton.Pressed += OnCommsButtonPressed;
		
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
		
		commsMenu.Closing += () =>
			Visible = true;
	}
}
