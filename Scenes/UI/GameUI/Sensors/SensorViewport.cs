using Godot;

namespace Spacelancer.Scenes.UI.GameUI.Sensors;

public partial class SensorViewport : SubViewportContainer
{
	private Camera2D _camera;
	private Node2D _target;
	
	public override void _Ready()
	{
		SetAnchorsAndOffsetsPreset(LayoutPreset.TopRight);
		
		_camera = GetNode<Camera2D>("%Camera2D");
		
		var subViewport = GetNode<SubViewport>("SubViewport");
		
		// Redirect the sub viewport to the main scene.
			// It lives in its own little context without this
		subViewport.World2D = GetViewport().World2D;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInstanceValid(_target))
		{
			SetCameraTarget(null);
			return;
		}
		
		if (_target != null)
			_camera.GlobalPosition = _target.GlobalPosition;
	}

	// May need to consider passing in the detection type and behave differently based on type
	// If following ships, we may need to consider having a process to follow the target
	public void SetCameraTarget(Node2D target)
	{
		_target = target;
		
		if (target == null)
		{
			Visible = false;
			return;
		}
		
		Visible = true;
		_camera.GlobalPosition = target.GlobalPosition;
	}
}