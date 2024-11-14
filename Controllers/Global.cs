using Godot;

namespace Spacelancer.Controllers;

/// <summary>
/// Auto-load class to enable us to manage controllers nicely
/// </summary>
public partial class Global : Node
{
    public static Global Instance;
    public static GameController GameController;
    public static EconomyController Economy { get; } = new();
    public static UniverseController Universe { get; } = new();
    public static Scenes.Player.Player Player;
    
    internal Logger Logger;
    internal SettingsController SettingsController;

    public override void _Ready()
    {
        if (Instance != null)
            return;

        Instance = this;

        // Logger needs to be loaded first, other things rely on it.
        Logger = Logger.Instance;
        SettingsController = SettingsController.Instance;

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