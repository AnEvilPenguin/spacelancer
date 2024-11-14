using Serilog;
using Serilog.Events;

namespace Spacelancer.Controllers.Settings;

/// <summary>
/// General settings. Log level, etc.
/// </summary>
internal class GeneralSettings : AbstractSettings
{
    internal override string Name { get => _name; }

    private static GeneralSettings _default = new GeneralSettings();

    private LogEventLevel _logLevel = LogEventLevel.Information;
    private const string _name = "General";

    public LogEventLevel LogLevel
    {
        get => _logLevel;
        set
        {
            _logLevel = value;
            
            Logger.LevelSwitch.MinimumLevel = _logLevel;

            Log.Debug($"Setting log level: {_logLevel}");
        }
    }

    internal GeneralSettings()
    {
    }
}