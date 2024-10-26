using Godot;

public partial class Station : Node2D
{
	public override void _Ready()
	{
		var stationBorder = GetNode<Area2D>("Area2D");
		
		stationBorder.BodyEntered += OnStationAreaEntered;
		stationBorder.BodyExited += OnStationAreaExited;
	}

	private void OnStationAreaEntered(Node2D body)
	{
		if (body is Player)
		{
			var label = Global.GameController.TempStationLabel;
			label.Text = $"Press C to talk to {Name}";
			label.Visible = true;
		}
	}

	private void OnStationAreaExited(Node2D body)
	{
		if (body is Player)
		{
			var label = Global.GameController.TempStationLabel;
			label.Visible = false;
		}
	}
}
