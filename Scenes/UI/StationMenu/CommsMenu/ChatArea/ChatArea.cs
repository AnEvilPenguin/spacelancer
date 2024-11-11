using Godot;
using System;
using Spacelancer.Scenes.UI.CommsMenu.ChatArea;

public partial class ChatArea : PanelContainer
{
	[Export]
	private PackedScene _chatMessageScene;
	
	private VBoxContainer _chatContainer;
	
	public override void _Ready()
	{
		_chatContainer = GetNode<VBoxContainer>("%ChatContainer");
	}

	public void SendMessage(string message, ChatDirection chatDirection)
	{
		var chatMessage = _chatMessageScene.Instantiate<ChatMessage>();
		chatMessage.SetMessage(message, chatDirection);
		_chatContainer.AddChild(chatMessage);
	}

	public void ClearMessages()
	{
		foreach (var child in _chatContainer.GetChildren())
		{
			child.QueueFree();
		}
	}
		
	
}
