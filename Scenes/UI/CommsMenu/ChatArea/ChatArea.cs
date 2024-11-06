using Godot;
using System;

public partial class ChatArea : PanelContainer
{
	private VBoxContainer _chatContainer;
	
	public override void _Ready()
	{
		_chatContainer = GetNode<VBoxContainer>("%ChatContainer");
	}
	
}
