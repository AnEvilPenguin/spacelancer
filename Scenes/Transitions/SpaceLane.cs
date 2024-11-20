using System.Collections.Generic;
using System.Linq;
using Godot;
using Spacelancer.Components.Equipment.Detection;

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
	[Export] private int _spacing
	{
		get => _spacingValue;
		set
		{
			_spacingValue = value; 
			_regenerate = true;
		}
	}
	private int _spacingValue = 1000;

	[Export] private int _ringCount 
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
	private Vector2 _offset
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
		
		var distance = _spacing * (_ringCount + 1);
		
		_pair1 = new LaneEntrance(Vector2.Zero, _offset, MainTexture, GoLight, StopLight);
		_pair2 = new LaneEntrance(new Vector2(distance, 0), _offset, MainTexture, GoLight, StopLight);

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

		for (int i = 1; i <= _ringCount; i++)
		{
			var position = new Vector2(i * _spacing, 0);
			var node = new LaneNode(position, IntermediateTexture, _offset);
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