using Godot;
using System;
using Serilog;

[Tool]
public partial class IconButton : CenterContainer
{
	[Signal]
	public delegate void PressedEventHandler();
	
	[Export]
	private Texture2D _iconTexture;
	
	[Export]
	private Color _iconColor = new Color(0.8f, 0.8f, 0.8f);
	
	private TextureButton _textureButton;
	private ShaderMaterial _material;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_textureButton = GetNode<TextureButton>("TextureButton");

		_textureButton.Pressed += OnButtonPressed;
		
		GenerateUniqueShaderMaterial();
			
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

	private void UpdateShader()
	{
		if (_iconTexture == null)
			return;

		_textureButton.TextureNormal = _iconTexture;
		
		_material.Set("shader_parameter/color1", _iconColor);
	}

	private void GenerateUniqueShaderMaterial()
	{
		if (_textureButton == null)
			return;
		
		_material = new ShaderMaterial();
		
		var shader = GD.Load("res://Scenes/UI/StationMenu/IconButton/icon_shader.gdshader").Duplicate(true) as Shader;
		_material.Shader = shader;
		
		_textureButton.Material = _material;
	}

	private void OnButtonPressed() =>
		EmitSignal(SignalName.Pressed);
}
