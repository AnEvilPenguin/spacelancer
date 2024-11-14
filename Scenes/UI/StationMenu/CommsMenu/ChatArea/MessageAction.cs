using Godot;
using Spacelancer.Components.NPCs;

namespace Spacelancer.Scenes.UI.StationMenu.CommsMenu.ChatArea;

public partial class MessageAction : PanelContainer
{
	[Signal]
	public delegate void SelectedEventHandler();
	
	private Response _response;
	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left })
			EmitSignal(SignalName.Selected);
	}

	public void SetMessageAction(Response response)
	{
		_response = response;
		
		var label = GetNode<Label>("Label");
		label.Text = _response.Text;
	}
}