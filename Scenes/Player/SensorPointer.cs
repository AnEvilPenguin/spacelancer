using Godot;

namespace Spacelancer.Scenes.Player;

[Tool]
public partial class SensorPointer : Node2D
{
	[Export]
	private int Distance
	{
		get => _distance;
		set
		{
			_distance = value;
			UpdateDistance();
		}
	}
	
	private int _distance;
	private Node2D _target;
	
	private Sprite2D _sprite;
	
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		UpdateDistance();

		Visible = false;
	}
	
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
			return;
		
		if (_target == null)
		{
			Visible = false;
			return;
		}
		
		var parent = GetParent<Node2D>();
		var position = parent.GlobalPosition;
		var targetPosition = _target.GlobalPosition;
		
		var direction =  targetPosition - position;

		// This caused soo much pain
		// Initially I rotated the component not the sprite so everything was off by 90 deg
		    // compared to the player where the sprite was rotated, not component.
		    // Orthogonal is a workaround, but a better bet is to check what has been rotated
		GlobalRotation = direction.Angle();
		
		Visible = direction.Length() > 400;
	}

	public void SetTarget(Node2D target)
	{
		_target = target;
		Visible = true;
	}
		

	public void ClearTarget()
	{
		_target = null;
		Visible = false;
	}

	private void UpdateDistance()
	{
		if (_sprite != null)
			_sprite.Position = new Vector2(_distance, 0);
	}
}