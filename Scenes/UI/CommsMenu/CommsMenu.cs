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
	private MessageArea _messageArea;
	
	private List<NonPlayerCharacter> _nonPlayerCharacters;
	private NonPlayerCharacter _currentNonPlayerCharacter;
	
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
		_messageArea = GetNode<MessageArea>("%MessageArea");

		_messageArea.ActionSelected += OnMessageAreaSelected;
	}

	public void LoadNonPlayerCharacters(List<NonPlayerCharacter> list)
	{
		_nonPlayerCharacters = list;

		try
		{
			_currentNonPlayerCharacter = _nonPlayerCharacters.First();
		}
		catch (Exception e)
		{
			Log.Error(e, "Unable to load first NPC");
			throw;
		}
		
		var dialog = _currentNonPlayerCharacter.GetDialog();
		NewInboundMessage(dialog);
	}

	private void NewInboundMessage(Dialog dialog)
	{
		_chatArea.SendMessage(dialog.Text, ChatDirection.Inbound);
		
		dialog.Responses.ForEach((r) => _messageArea.AddMessageAction(r));
	}

	private void OnMessageAreaSelected(string text, int nextId)
	{
		_chatArea.SendMessage(text, ChatDirection.Outbound);
		
		var nextDialog = _currentNonPlayerCharacter.GetDialog(nextId);
		NewInboundMessage(nextDialog);
	}
}
