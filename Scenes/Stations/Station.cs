using Godot;
using Serilog;

public partial class Station : Node2D
{
	// We may need to consider making this docking range if we make a map and remote comms or something
	private bool _playerInCommsRange = false;
	StationMenu _menu;
	
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

		if (Input.IsKeyPressed(Key.C) && (_menu == null || !_menu.Visible))
		{
			Log.Debug("Comms initiated with {StationName}", Name);
			_menu = Global.GameController.LoadScene<StationMenu>("res://Scenes/UI/StationMenu/station_menu.tscn");
			_menu.ShowMenu();
		}
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
