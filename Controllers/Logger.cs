using System.IO;
using Serilog;
using Serilog.Core;
using Constants = Spacelancer.Util.Constants;

namespace Spacelancer.Controllers;

/// <summary>
/// Configures and closes down the log system.
/// </summary>
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

    static Logger()
    {}

    private Logger() 
    {
        var logPath = Path.Combine(
            Constants.FolderPath,
            "spacelancer-.log"
        );

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LevelSwitch)
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("The global logger has been configured");
    }

    internal void StopLogger()
    {
        Log.Information("Quit Notification received. Closing Logger.");

        Log.CloseAndFlush();
    }
}