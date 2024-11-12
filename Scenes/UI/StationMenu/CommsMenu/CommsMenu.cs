using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Serilog;
using Spacelancer.Components.NPCs;
using Spacelancer.Scenes.UI.StationMenu.CommsMenu.ChatArea;

namespace Spacelancer.Scenes.UI.StationMenu.CommsMenu;

public partial class CommsMenu : PanelContainer
{
	private ChatArea.ChatArea _chatArea;
	private ChatArea.MessageArea _messageArea;
	
	private List<NonPlayerCharacter> _nonPlayerCharacters;
	private NonPlayerCharacter _currentNonPlayerCharacter;
	
	public override void _Ready()
	{
		_chatArea = GetNode<ChatArea.ChatArea>("%ChatArea");
		_messageArea = GetNode<ChatArea.MessageArea>("%MessageArea");

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

	public void ClearChat() =>
		_chatArea.ClearMessages();

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