using Serilog;
using Serilog.Core;
using System.IO;

internal sealed class Logger
{
    public static Logger Instance
    {
        get
        {
            return _instance;
        }
    }

    public static LoggingLevelSwitch LevelSwitch = new LoggingLevelSwitch();

    private static readonly Logger _instance = new Logger();
    private static string _logPath;

    static Logger()
    {}

    private Logger() 
    {
        _logPath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "EvilPenguinIndustries\\Spacelancer",
            "spacelancer-.log"
        );

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LevelSwitch)
            .WriteTo.File(_logPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("The global logger has been configured");
    }

    internal void StopLogger()
    {
        Log.Information("Quit Notification received. Closing Logger.");

        Log.CloseAndFlush();
    }
}