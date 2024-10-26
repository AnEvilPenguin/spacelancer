using Godot;
using Serilog;

public partial class Station : Node2D
{
	// We may need to consider making this docking range if we make a map and remote comms or something
	private bool _playerInCommsRange = false;
	
	public override void _Ready()
	{
		var stationBorder = GetNode<Area2D>("Area2D");
		
		stationBorder.BodyEntered += OnStationAreaEntered;
		stationBorder.BodyExited += OnStationAreaExited;
	}

	public override void _Process(double delta)
	{
		if (!_playerInCommsRange)
			return;
		
		if (Input.IsKeyPressed(Key.C))
			Log.Debug("Comms initiated with {StationName}", Name); // TODO load station menu instead.
	}

	private void OnStationAreaEntered(Node2D body)
	{
		if (body is Player)
		{
			_playerInCommsRange = true;
			Log.Debug("Player in comms range of {StationName}", Name);
			
			var label = Global.GameController.TempStationLabel;
			label.Text = $"Press C to talk to {Name}";
			label.Visible = true;
		}
	}

	private void OnStationAreaExited(Node2D body)
	{
		if (body is Player)
		{
			_playerInCommsRange = false;
			Log.Debug("Player left comms range of {StationName}", Name);
			
			var label = Global.GameController.TempStationLabel;
			label.Visible = false;
		}
	}
}
