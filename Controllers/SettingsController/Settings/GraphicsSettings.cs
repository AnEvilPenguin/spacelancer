using Godot;
using Serilog;

/// <summary>
/// Settings related to graphics, FullScreen, Resolution, etc.
/// </summary>
internal class GraphicsSettings : AbstractSettings
{
    internal override string Name { get => _name; }

    private static GraphicsSettings _default = new GraphicsSettings();

    private bool _fullScreen = true;
    private const string _name = "Graphics";

    public bool FullScreen
    {
        get => _fullScreen;
        set
        {
            _fullScreen = value;

            var mode = _fullScreen ?
                DisplayServer.WindowMode.Fullscreen :
                DisplayServer.WindowMode.Windowed;

            DisplayServer.WindowSetMode(mode);

            Log.Debug($"Setting DisplayServer Window Mode: {mode}");
        }
    }

    internal GraphicsSettings()
    {
    }
}
