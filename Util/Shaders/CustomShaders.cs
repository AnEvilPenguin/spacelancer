using Godot;

namespace Spacelancer.Util.Shaders;

public static class CustomShaders
{
    private static readonly Resource SimpleColorShader = GD.Load("res://Util/Shaders/simple_color_shader.gdshader");

    public static ShaderMaterial GetSimpleColorShader()
    {
        var shader = SimpleColorShader.Duplicate(true) as Shader;
        
        var material = new ShaderMaterial();
        material.Shader = shader;
        
        return material;
    }
}