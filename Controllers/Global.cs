using Godot;
using Spacelancer.Scenes.SpaceShips;

namespace Spacelancer.Controllers;

/// <summary>
/// Auto-load class to enable us to manage controllers nicely
/// </summary>
public partial class Global : Node
{
    public static bool IsClosing = false;
    
    public static Global Instance;
    public static GameController GameController;
    public static EconomyController Economy { get; } = new();
    public static UniverseController Universe { get; } = new();
    
    public static SystemController SolarSystem { get; } = new();
    public static UiController UserInterface { get; private set; }
    public static Player Player;
    
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

        UserInterface = new UiController();

        GetTree().AutoAcceptQuit = false;
    }

    public override void _Notification(int what)
    {
        if (what != NotificationWMCloseRequest)
            return;

        // consider pausing game?
        IsClosing = true;
        
        if (GameController != null)
            GameController.UnloadWorld2D();

        if (Logger != null)
            Logger.StopLogger();
        
        GetTree().Quit();
    }
}