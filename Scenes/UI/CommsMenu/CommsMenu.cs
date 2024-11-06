using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Spacelancer.Components.NPCs;
using Spacelancer.Scenes.UI.CommsMenu.ChatArea;

public partial class CommsMenu : PanelContainer
{
	[Signal]
	public delegate void ClosingEventHandler();
	
	private ChatArea _chatArea;
	
	private List<NonPlayerCharacter> _nonPlayerCharacters;
	
	public override void _Ready()
	{
		var leaveButton = GetNode<Button>("%LeaveButton");
		
		leaveButton.Pressed += () =>
		{
			Visible = false;
			_chatArea.ClearMessages();
			EmitSignal(SignalName.Closing);
		};
		
		_chatArea = GetNode<ChatArea>("%ChatArea");
	}

	public void LoadNonPlayerCharacters(List<NonPlayerCharacter> list)
	{
		_nonPlayerCharacters = list;

		NonPlayerCharacter first;

		try
		{
			first = _nonPlayerCharacters.First();
		}
		catch (Exception e)
		{
			Log.Error(e, "Unable to load first NPC");
			throw;
		}
		
		var dialog = first.GetDialog();
		_chatArea.SendMessage(dialog.Text, ChatDirection.Inbound);
		
		// TODO get some sort of loop where we can get and set the next response?
	}
}
