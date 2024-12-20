using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Spacelancer.Scenes.Transitions;

[Tool]
public partial class SpaceLane : Node2D
{
	[ExportGroup("Textures")]
	[Export]
	public Texture2D MainTexture { get; private set; }
	[Export]
	public Texture2D IntermediateTexture { get; private set; }
	[Export]
	public Texture2D GoLight { get; private set; }
	[Export]
	public Texture2D StopLight { get; private set; }

	[ExportGroup("Generation")] 
	[Export] 
	private int Spacing
	{
		get => _spacingValue;
		set
		{
			_spacingValue = value; 
			_regenerate = true;
		}
	}
	private int _spacingValue = 1000;

	[Export] 
	private int RingCount 
	{ 
		get => _ringCountValue;
		set {
			_ringCountValue = value;
			_regenerate = true; 
		}
	}

	private int _ringCountValue = 0;
	
	private List<LanePart> _laneParts = new();
	
	private LaneEntrance _pair1;
	private LaneEntrance _pair2;

	[Export]
	private Vector2 Offset
	{
		get => _offsetValue;
		set
		{
			_offsetValue = value;
			_regenerate = true;
		}
	} 
	private Vector2 _offsetValue = new(150, 0);

	private bool _regenerate = true;
	
	// TODO docking area
	// TODO navigation software
	// TODO Get Names From IDs?
		// If stationIds has name use that
		// else if JumpGates has id use that
		// else use name

	public override void _Ready()
	{
		UpdateNodes();
	}

	public override void _Process(double delta)
	{
		if (!Engine.IsEditorHint() || !_regenerate)
			return;
		
		UpdateNodes();
	}

	public Tuple<Vector2, Vector2> GetNavigationPositions(Vector2 globalPosition)
	{
		var d1 = globalPosition.DistanceTo(_pair1.GlobalPosition);
		var d2 = globalPosition.DistanceTo(_pair2.GlobalPosition);
		
		var tuple = d1 < d2 ?
			new Tuple<Vector2, Vector2>(_pair1.GlobalPosition, _pair2.GlobalPosition) :
			new Tuple<Vector2, Vector2>(_pair2.GlobalPosition, _pair1.GlobalPosition);
		
		return tuple;
	}

	public LaneEntrance GetNearestEntrance(Vector2 globalPosition) =>
		globalPosition.DistanceTo(_pair1.GlobalPosition) < globalPosition.DistanceTo(_pair2.GlobalPosition) ? 
			_pair1 : _pair2;

	private void UpdateNodes()
	{
		Regenerate();
		
		RotateAway(_pair1, _pair2);

		if (Engine.IsEditorHint())
		{
			GD.Print("Updated nodes");
			DebugLabels();
		}
	}

	private void Regenerate()
	{
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
		_laneParts.Clear();
		
		var distance = Spacing * (RingCount + 1);
		
		_pair1 = new LaneEntrance(Vector2.Zero, Offset, MainTexture, GoLight, StopLight);
		_pair2 = new LaneEntrance(new Vector2(distance, 0), Offset, MainTexture, GoLight, StopLight);

		// FIXME we need a better way of dealing with names
		// Take two 'ids' and try to resolve them. If not resolved treat as display name instead.
		_pair1.Name = Name;
		_pair2.Name = String.Join(" -> ", Name.ToString().Split(" -> ").Reverse());

		_pair1.Partner = _pair2;
		_pair2.Partner = _pair1;
		
		AddChild(_pair1);
		AddChild(_pair2);
		
		RotateAway(_pair1, _pair2);
		
		GenerateParts();
		
		_regenerate = false;
	}

	private void RotateAway(Node2D object1, Node2D object2)
	{
		var vector1 = object1.Position - object2.Position;
		var vector2 = object2.Position - object1.Position;
		
		var rot1 = vector1.Orthogonal().Angle();
		var rot2 = vector2.Orthogonal().Angle();
		
		object1.Rotation = rot2;
		object2.Rotation = rot1;
	}

	private void GenerateParts()
	{
		_laneParts.Add(_pair1);

		LanePart previous = _pair1;

		for (int i = 1; i <= RingCount; i++)
		{
			var position = new Vector2(i * Spacing, 0);
			var node = new LaneNode(position, IntermediateTexture, Offset);
			node.Rotation = _pair1.Rotation;
			
			_laneParts.Add(node);
			AddChild(node);
			
			node.TowardsPair1 = previous;
			previous.TowardsPair2 = node;

			previous = node;
		}
		
		previous.TowardsPair2 = _pair2;
		_pair2.TowardsPair1 = previous;
		
		_laneParts.Add(_pair2);
		
		
		// split position down into batches of 1000 length
		// Generate a pair of intermediaries per batch
		// Place batch
		// link neighbours together
			// towardsPair1
			// towardsPair2
	}

	private void DebugLabels()
	{
		var label1 = GetDebugLabel(_pair1);
		var label2 = GetDebugLabel(_pair2);
		
		var distance = (_pair1.Position - _pair2.Position).Length();
		
		label1.Text = $"distance: {distance:0.0}";
		label2.Text = $"distance: {distance:0.0}";
	}
	
	private Label GetDebugLabel(Node2D node)
	{
		var label = node.GetNodeOrNull<Label>("DebugLabel");
		if (label == null)
		{
			label = new Label();
			label.Name = "DebugLabel";
			node.AddChild(label);
		}
		
		label.RotationDegrees = 90;
		label.Position = new Vector2(12, -50);
		
		return label;
	}

	public string Detect(Node2D callee)
	{
		throw new System.NotImplementedException();
	}
}