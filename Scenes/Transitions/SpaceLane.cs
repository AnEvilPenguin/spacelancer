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
	
	private Node2D _pair1;
	private Node2D _pair2;

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
		
		if (_pair1.Position == Position1 && _pair2.Position == Position2)
			return;
		
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
		if(_pair1 != null)
			_pair1.QueueFree();
		
		if (_pair2 != null)
			_pair2.QueueFree();
		
		_pair1 = GeneratePair(Position1, "pair1");
		_pair2 = GeneratePair(Position2, "pair2");
		
		AddChild(_pair1);
		AddChild(_pair2);
		
		RotateAway(_pair1, _pair2);
		
		_regenerate = false;
	}

	private Node2D GeneratePair(Vector2 position, string name)
	{
		var pair = new Node2D();
		pair.Position = position;
		pair.Name = name;

		var offset = new Vector2(150, 0);
		
		var entrance = GenerateEntrance(offset, "entrance");
		var exit = GenerateExit(-offset, "exit");
		
		pair.AddChild(entrance);
		pair.AddChild(exit);
		
		return pair;
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
		var label1 = GetDebugLabel(_pair1);
		var label2 = GetDebugLabel(_pair2);
		
		var distance = (_pair1.Position - _pair2.Position).Length();
		
		// TODO if distance not a multiple of 1000 (2000?) make labels red and log a GD error
		
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