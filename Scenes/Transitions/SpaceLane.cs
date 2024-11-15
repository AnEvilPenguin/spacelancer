using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Spacelancer.Scenes.Transitions;

[Tool]
public partial class SpaceLane : Node2D
{
	[Export]
	public Vector2 Position1 { get; private set; }
	[Export]
	public Vector2 Position2 { get; private set; }
	[Export]
	public Texture2D MainTexture { get; private set; }
	[Export]
	public Texture2D IntermediateTexture { get; private set; }
	[Export]
	public Texture2D GoLight { get; private set; }
	[Export]
	public Texture2D StopLight { get; private set; }

	[Export] 
	private int Spacing = 1000;
	
	private List<LanePart> _laneParts = new();
	
	private LaneEntrance _pair1;
	private LaneEntrance _pair2;
	
	[Export]
	private Vector2 _offset = new(150, 0);

	[Export] private bool _regenerate = true;
	
	// TODO docking area
	// TODO navigation software
	// TODO Get Names From IDs?
	
	// FIXME consider just specifying number of rings to set distance
	// Rotation effectvely handled by the parent node
	
	// FIXME split down into logical classes
	// FIXME look at setters and getters to implement export logic rather than tool class
	// https://docs.godotengine.org/en/latest/tutorials/scripting/gdscript/gdscript_basics.html#properties-setters-and-getters
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateNodes();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Engine.IsEditorHint())
			return;

		if (_regenerate || _pair1.Position != Position1 || _pair2.Position != Position2)
			UpdateNodes();
	}

	private void UpdateNodes()
	{
		Regenerate();
		
		_pair1.Position = Position1;
		_pair2.Position = Position2;
		
		RotateAway(_pair1, _pair2);

		if (Engine.IsEditorHint())
		{
			GD.Print("Updated nodes");
			DebugLabels();
		}
	}

	private void Regenerate()
	{
		_laneParts.ForEach(p => p.QueueFree());
		_laneParts.Clear();
		
		_pair1 = new LaneEntrance(Position1, _offset, MainTexture, GoLight, StopLight);
		_pair2 = new LaneEntrance(Position2, _offset, MainTexture, GoLight, StopLight);
		
		AddChild(_pair1);
		AddChild(_pair2);
		
		RotateAway(_pair1, _pair2);

		GenerateIntermediates();
		
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

	private void GenerateIntermediates()
	{
		var distance = _pair2.Position - _pair1.Position;

		_laneParts.Add(_pair1);

		LanePart previous = _pair1;

		while (distance.Length() > Spacing)
		{
			var chunk = distance.LimitLength(Spacing);

			var newPosition = previous.Position + chunk;
			var node = new LaneNode(newPosition, IntermediateTexture, _offset);
			node.Rotation = _pair1.Rotation;
			
			_laneParts.Add(node);
			AddChild(node);
			
			node.TowardsPair1 = previous;
			previous.TowardsPair2 = node;

			previous = node;
			distance -= chunk;
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
		
		label1.AddThemeColorOverride("font_color", Colors.White);
		label2.AddThemeColorOverride("font_color", Colors.White);
		
		// We're happy with some inaccuracy here.
		if ((int)distance % Spacing != 0)
		{
			GD.PrintErr($"Spacing mismatch for {Name}");
			label1.AddThemeColorOverride("font_color", Colors.Red);
			label2.AddThemeColorOverride("font_color", Colors.Red);
		}
		
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
}