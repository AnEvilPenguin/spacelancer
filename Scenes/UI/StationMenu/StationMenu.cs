using Godot;
using System;

public partial class StationMenu : CenterContainer
{
	private Station _selectedStation;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var leaveButton = GetNode<Button>("%Leave");
		leaveButton.Pressed += OnLeaveButtonPressed;

		var tradeButton = GetNode<Button>("%Trade");
		tradeButton.Pressed += OnTradeButtonPressed;
		
		// We can interact with this menu when the game is paused in the background.
		// We might not need this when we actually implement docking
		ProcessMode = ProcessModeEnum.Always;
	}

	public void ShowMenu(Station station)
	{
		Visible = true;
		GetTree().Paused = true;
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
		
		tradeMenu.SetStation(_selectedStation);
		tradeMenu.Closing += () =>
			Visible = true;
	}
}
