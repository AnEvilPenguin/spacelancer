using Godot;
using System;
using Spacelancer.Components.NPCs;

public partial class MessageArea : PanelContainer
{
	[Signal]
	public delegate void ActionSelectedEventHandler(string text, int next);
	
	[Export]
	private PackedScene _actionScene;
	
	private VBoxContainer _actionArea;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_actionArea = GetNode<VBoxContainer>("VBoxContainer");
		ClearMessageActions();
	}

	public void ClearMessageActions()
	{
		var children = _actionArea.GetChildren();
		foreach (var child in children)
		{
			child.QueueFree();
		}
	}

	public void AddMessageAction(Response response)
	{
		var action = _actionScene.Instantiate<MessageAction>();
		action.SetMessageAction(response);

		action.Selected += () =>
		{
			ClearMessageActions();
			EmitSignal(SignalName.ActionSelected, response.Text, response.Next);
		};
		
		_actionArea.AddChild(action);
	}
}
