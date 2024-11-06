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

		var test1 = _chatMessageScene.Instantiate<ChatMessage>();
		test1.SetMessage("Cat ipsum dolor sit amet, kitty time scratch me now! stop scratching me! check cat door for ambush 10 times before coming in so ask to go outside and ask to come inside and ask to go outside and ask to come inside. Meow loudly just to annoy owners steal the warm chair right after you get up bleghbleghvomit my furball really tie the room together for paw at beetle and eat it before it gets away and do doodoo in the litter-box, clickityclack on the piano, be frumpygrumpy.", ChatDirection.Inbound);
		_chatContainer.AddChild(test1);
		
		var test2 = _chatMessageScene.Instantiate<ChatMessage>();
		test2.SetMessage("Bacon ipsum dolor amet alcatra chuck chislic jowl beef ribs burgdoggen. Doner meatball hamburger salami tenderloin chuck ribeye bacon", ChatDirection.Outbound);
		_chatContainer.AddChild(test2);
	}
	
}
