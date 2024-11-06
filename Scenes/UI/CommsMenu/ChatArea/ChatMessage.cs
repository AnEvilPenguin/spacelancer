using Godot;
using System;
using Spacelancer.Scenes.UI.CommsMenu.ChatArea;

public partial class ChatMessage : PanelContainer
{
	[Export]
	private Color _inboundColor;
	[Export]
	private Color _outboundColor;

	public void SetMessage(string message, ChatDirection direction)
	{
		var colorRect = GetNode<ColorRect>("ColorRect");

		if (direction == ChatDirection.Inbound)
		{
			colorRect.Color = _inboundColor;
			SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
		}
		else
		{
			colorRect.Color = _outboundColor;
			SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
		}
		
		var chatMessage = GetNode<RichTextLabel>("RichTextLabel");
		chatMessage.Text = message;
		
		// TODO consider using a timer to make the message 'type out' with VisibleCharacters
		// TODO when we move to themes consider #CCCCCC as the text color
	}
}
