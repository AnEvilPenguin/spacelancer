using Godot;
using System;

public partial class TradeMenu : CenterContainer
{
	[Signal]
	public delegate void ClosingEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var leaveButton = GetNode<Button>("%LeaveButton");
		
		leaveButton.Pressed += () =>
		{
			Visible = false;
			EmitSignal(SignalName.Closing);
			QueueFree();
		};
	}
}
