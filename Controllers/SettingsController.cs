using System.IO;
using Newtonsoft.Json.Linq;
using Serilog;
using Spacelancer.Controllers.SettingsController.Settings;
using Spacelancer.Util;

namespace Spacelancer.Controllers.SettingsController;

/// <summary>
/// Manages the saving and loading of settings from file (or not).
/// </summary>
internal sealed class SettingsController
{
    private const string _fileName = "settings.json";

    private static GraphicsSettings _graphics = new GraphicsSettings();
    private static GeneralSettings _general = new GeneralSettings();
    public static SettingsController Instance
    {
        get
        {
            return _instance;
        }
    }

    private static readonly SettingsController _instance = new SettingsController();

    static SettingsController()
    { }

    private SettingsController()
    {
        Log.Debug("Created SettingsController. Loading initial settings.");

        Load();
    }

    // TODO Move file loading to util file 
    public static void Load()
    {
        var fullPath = Path.Combine(Constants.FolderPath, _fileName);

        // Nothing to load, just use defaults
        if (!File.Exists(fullPath))
        {
            Log.Debug("Using default settings");
            return;
        }

        var jsonString = File.ReadAllText(fullPath);

        if (string.IsNullOrWhiteSpace(jsonString))
        {
            Log.Error("Empty settings file");
            return;
        }

        var settingOverrides = JObject.Parse(jsonString);

        _general.OverrideSettings(settingOverrides);
        _graphics.OverrideSettings(settingOverrides);
    }

    // TODO generate tab per class (or ask the class to do it)
    // TODO generate components per setting type.
}