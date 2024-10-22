using Godot;

/// <summary>
/// Auto-load class to enable us to manage controllers nicely
/// </summary>
public partial class Global : Node
{
    public static Global Instance;

    public GameController GameController;
    internal Logger Logger;

    public override void _Ready()
    {
        if (Instance != null)
            return;

        Instance = this;

        Logger = Logger.Instance;

        GetTree().AutoAcceptQuit = false;
    }

    public override void _Notification(int what)
    {
        if (what != NotificationWMCloseRequest)
            return;

        Logger.StopLogger();
        GetTree().Quit();
    }
}
