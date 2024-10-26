using Godot;
using System;

public partial class StationMenu : CenterContainer
{
	private Button _leaveButton; 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_leaveButton = GetNode<Button>("%Leave");
		_leaveButton.Pressed += OnLeaveButtonPressed;
		
		// We can interact with this menu when the game is paused in the background.
		// We might not need this when we actually implement docking
		ProcessMode = ProcessModeEnum.Always;
	}

	public void ShowMenu()
	{
		Visible = true;
		GetTree().Paused = true;
	}

	private void OnLeaveButtonPressed()
	{
		Visible = false;
		GetTree().Paused = false;
	}
}
