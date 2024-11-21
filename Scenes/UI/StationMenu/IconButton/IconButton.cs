using System;
using Godot;
using Serilog;
using Spacelancer.Util.Shaders;

namespace Spacelancer.Scenes.UI.StationMenu.IconButton;

[Tool]
public partial class IconButton : CenterContainer
{
	[Signal]
	public delegate void PressedEventHandler();
	
	[Export]
	public bool Disabled { get; set; } = false;
	
	[Export]
	private Texture2D _iconTexture;
	
	[Export]
	private Color _iconColor = new Color(0.8f, 0.8f, 0.8f);
	
	private TextureButton _textureButton;
	private readonly ShaderMaterial _material = CustomShaders.GetSimpleColorShader();

	public override void _Ready()
	{
		_textureButton = GetNode<TextureButton>("TextureButton");

		_textureButton.Pressed += OnButtonPressed;
		_textureButton.Material = _material;
			
		try
		{
			UpdateShader();
		}
		catch (Exception e)
		{
			Log.Error(e, "Unable to set texture icon for {Name}", Name);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Engine.IsEditorHint())
			return;
		
		// We want errors to bubble up in the editor
		UpdateShader();
	}

	public void SetColor(Color color)
	{
		_iconColor = color;
		_material.Set("shader_parameter/color1", _iconColor);
	}

	private void UpdateShader()
	{
		if (_iconTexture == null)
			return;

		_textureButton.TextureNormal = _iconTexture;
		
		_material.Set("shader_parameter/color1", _iconColor);
	}

	private void OnButtonPressed()
	{
		if (Disabled)
			return;
		
		EmitSignal(SignalName.Pressed);
	}
		
}