using Godot;
using System;

public partial class TradeMenu : CenterContainer
{
	[Signal]
	public delegate void ClosingEventHandler();
	
	private Station _selectedStation;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var leaveButton = GetNode<Button>("%LeaveButton");
		
		leaveButton.Pressed += () =>
		{
			Visible = false;
			EmitSignal(SignalName.Closing);
		};
		
		// TODO get player inventory and add them to the list.
		// TODO get station inventory and add it to the list.
	}
	
	public void SetStation(Station station) =>
		_selectedStation = station;
}
