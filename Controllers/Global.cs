using Godot;

/// <summary>
/// Auto-load class to enable us to manage controllers nicely
/// </summary>
public partial class Global : Node
{
    public static Global Instance;

    public GameController GameController;

    public override void _Ready()
    {
        if (Instance != null)
            return;

        Instance = this;
    }
}
