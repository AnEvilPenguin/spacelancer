using Godot;

namespace Spacelancer.Scenes.UI.StationMenu.CommsMenu.ChatArea;

public partial class ChatMessage : PanelContainer
{
	[Export]
	private Color _inboundColor;
	[Export]
	private Color _outboundColor;

	[Export] 
	private int _maximumWidth = 500;
	
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
		
		// Needed because of issues sizing RichTextLabels
		// Without it we need to set a minimum size for the component that acts as the maximum for the label
		// But this looks super silly for very small messages.
		BodgeComponentSize(message);
		
		var chatMessage = GetNode<RichTextLabel>("RichTextLabel");
		chatMessage.Text = message;
		
		// TODO consider using a timer to make the message 'type out' with VisibleCharacters
		// TODO when we move to themes consider #CCCCCC as the text color
	}

	private void BodgeComponentSize(string message)
	{
		var label = GetNode<Label>("KludgeLabel");
		label.Text = message;

		var x = label.GetMinimumSize().X;
		
		Vector2 minimumSize = label.GetMinimumSize().X < _maximumWidth ? 
			new Vector2(x, 0) : new Vector2(_maximumWidth, 0);
		
		CustomMinimumSize = minimumSize;
		
		label.Visible = false;
	}
}