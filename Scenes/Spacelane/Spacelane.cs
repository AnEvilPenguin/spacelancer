using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Spacelancer.Components.Navigation;

public partial class Spacelane : Node2D
{
	[Export]
	public Spacelane Partner { get; private set; }

	private Area2D _border;
	private Dictionary<ulong, LaneNavigation> _travelers = new Dictionary<ulong, LaneNavigation>();
	
	// TODO navigation software to get into lane and through to destination, and then out of the lane.
		// Move to origin
		// Stop
		// Rotate to Destination
		// Move at Speed X to destination
		// Hand over control to Destination
		// For future, queueing and/or handling premature drop out.
	
	public override void _Ready()
	{
		_border = GetNode<Area2D>("Area2D");

		_border.BodyEntered += OnLaneBorderEntered;
		_border.BodyExited += OnLaneBorderExited;
	}

	public void AddTraveller(Player player, LaneNavigation computer)
	{
		_travelers.Add(player.GetInstanceId(), computer);
	}

	private void OnLaneBorderEntered(Node2D body)
	{
		Log.Debug("{Body} entered space lane {Origin} to {Destination}", body.Name, Name, Partner.Name);

		if (body is Player player)
		{
			TakeControlOfShip(player);
		}
	}

	private void OnLaneBorderExited(Node2D body)
	{
		Log.Debug("{Body} exited space lane {Here}", body.Name, Name);
		
		var id = body.GetInstanceId();

		if (_travelers.ContainsKey(id))
		{
			var computer = _travelers[body.GetInstanceId()];
			computer.ExitLane();
			_travelers.Remove(id);
		}
	}

	private void TakeControlOfShip(Player player)
	{
		if (player.NavComputer is LaneNavigation navigation)
			return;
		
		var computer = new LaneNavigation(player, this, Partner);
		player.NavComputer = computer;
		Partner.AddTraveller(player, computer);
	}
}