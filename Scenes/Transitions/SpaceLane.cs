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

	private Node2D _entrance1;
	private Node2D _exit1;

	[Export]
	private bool _ready = false;

	[Export] private bool _regenerate = true;
	
	// TODO Debug Distance labels
	// TODO Place intermediate Rings
	// TODO Get Names From IDs?
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Regenerate();
		UpdateNodes();
		
		_ready = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Engine.IsEditorHint())
			return;
			
		UpdateNodes();
		_ready = true;
	}

	private void UpdateNodes()
	{
		if(_regenerate)
			Regenerate();
		
		if (_entrance1.Position == Position1 && _exit1.Position == Position2)
			return;
		
		_entrance1.Position = Position1;
		_exit1.Position = Position2;
		
		RotateAway(_entrance1, _exit1);

		if (Engine.IsEditorHint())
		{
			GD.Print("Updated nodes");
			DebugLabels();
		}
	}

	private void Regenerate()
	{
		if(_entrance1 != null)
			_entrance1.QueueFree();
		
		if (_exit1 != null)
			_exit1.QueueFree();
		
		_entrance1 = GenerateEntrance(Position1, "entrance1");
		_exit1 = GenerateExit(Position2, "entrance2");
		
		RotateAway(_entrance1, _exit1);
		
		_regenerate = false;
	}

	private Node2D GenerateEntrance(Vector2 position, string name)
	{
		var node = GenerateMainNode(position, name);
		
		GenerateLights(node, new Vector2(110, 0), GoLight);
		
		return node;
	}

	private Node2D GenerateExit(Vector2 position, string name)
	{
		var node = GenerateMainNode(position, name);
		
		GenerateLights(node, new Vector2(110, 0), StopLight);
		
		return node;
	}

	private void GenerateLights(Node2D node, Vector2 position, Texture2D lightTexture)
	{
		node.AddChild(GenerateLight(position, lightTexture));
		node.AddChild(GenerateLight(-position, lightTexture));
	}

	private Sprite2D GenerateLight(Vector2 position, Texture2D texture)
	{
		var sprite = new Sprite2D();
		sprite.Texture = texture;
		sprite.ZIndex = -2;
		sprite.Position = position;
		
		return sprite;
	}

	private Node2D GenerateMainNode(Vector2 position, string name)
	{
		var newNode = new Node2D();
		newNode.Position = position;
		newNode.Name = name;

		var sprite = new Sprite2D();
		sprite.Texture = MainTexture;
		
		newNode.AddChild(sprite);
		AddChild(newNode);
		
		return newNode;
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
		
	}

	private void DebugLabels()
	{
		var label1 = GetDebugLabel(_entrance1);
		var label2 = GetDebugLabel(_exit1);
		
		var distance = (_entrance1.Position - _exit1.Position).Length();
		
		GD.Print($"distance: {distance:0.0}");
		
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
		label.Position = new Vector2(175, -50);
		
		return label;
	}
}