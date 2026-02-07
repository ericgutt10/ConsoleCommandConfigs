using System.Reflection;

namespace ConsoleTemplate.Lib;

public sealed class AppConfig
{
    // ---------------
    // Global Constants
    // ---------------

    // Compile-time constants (const)
    public const string COMMANDS = "Commands";

    // Runtime-initialized constants (static readonly)
    public static readonly string AppVersion;

    public static readonly DateTime BuildDate;

    // ---------------
    // Singleton Initialization (Lazy<T> for thread safety)
    // ---------------
    private static readonly Lazy<AppConfig> _lazyInstance = new(() => new AppConfig());

    // Private constructor: Initializes runtime constants
    private AppConfig()
    {
        if (_lazyInstance.IsValueCreated) // Check if instance already exists
        {
            throw new InvalidOperationException("AppConfig instance already created.");
        }
    }

    // Static constructor: Initializes static readonly fields (runs once)
    static AppConfig()
    {
        // Load app version from assembly metadata
        AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

        // Load build date from the executable's creation time
        var assemblyPath = Assembly.GetExecutingAssembly().Location;
        BuildDate = new FileInfo(assemblyPath).CreationTime;
    }

    // Public accessor
    public static AppConfig Instance => _lazyInstance.Value;

    // ---------------
    // Global Functions
    // ---------------

    /// <summary>Validates an API key (example utility function)</summary>
    public static bool IsValidApiKey(string apiKey) =>
        !string.IsNullOrWhiteSpace(apiKey) && apiKey.Length == 32; // Example: 32-char key

    /// <summary>Formats a log message with a timestamp</summary>
    public static string FormatLogEntry(string message) =>
        $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {message}";
}