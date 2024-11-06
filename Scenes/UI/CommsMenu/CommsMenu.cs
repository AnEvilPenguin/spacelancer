using Godot;
using System;

public partial class CommsMenu : PanelContainer
{
	[Signal]
	public delegate void ClosingEventHandler();
	
	public override void _Ready()
	{
		var leaveButton = GetNode<Button>("%LeaveButton");
		
		leaveButton.Pressed += () =>
		{
			Visible = false;
			EmitSignal(SignalName.Closing);
		};
	}
}
