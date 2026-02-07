using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Settings.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ConsoleTemplate.Lib;

internal static class HostAssy
{
    private static string? _typName;
    private static string? _logFullPath;
    private static Assembly? _assy;
    private static AssemblyConfigurationAttribute? _assyAttr;
    private static DirectoryInfo? _logDir;
    private static readonly string? _logFileName;

    static HostAssy()
    {
        _logFileName = $"-{DateTime.Now:yyyy-MM-dd-HHmmssffff}.log";
    }

#pragma warning disable CS0649 // Field 'HostAssy._configName' is never assigned to, and will always have its default value null
    private static readonly string? _configName;
#pragma warning restore CS0649 // Field 'HostAssy._configName' is never assigned to, and will always have its default value null

    public static Assembly Host<T>() where T : class => _assy ??= typeof(T).Assembly;

    public static AssemblyConfigurationAttribute? HostConfigAttr<T>() where T : class =>
        _assyAttr ??= typeof(T)?.Assembly?.GetCustomAttribute<AssemblyConfigurationAttribute>();

    public static string? HostConfigName<T>() where T : class => _configName ?? HostConfigAttr<T>()?.Configuration;

    public static string? HostName<T>() where T : class => _typName ??= Host<T>().GetName().Name;

    [RequiresUnreferencedCode("")]
    public static string LogPath<T>(IConfiguration configuration) where T : class => _logFullPath ??= (
            _logDir ??= new DirectoryInfo( (configuration.GetValue<string?>("AppSettings:LogsDirectory") switch
            {
                string p when !string.IsNullOrWhiteSpace(p) => Path.Combine(p, $"{HostName<T>()}{_logFileName}"),
                _ => Path.Combine(Path.GetTempPath(), $"{HostName<T>()}", $"{HostName<T>()}{_logFileName}")
            }))).FullName;
}

internal static class CreateConfigurationFactory
{
    private static IConfiguration? _config;

    public static IConfiguration CreateConfiguration<T>(this
        IHostBuilder _,
        bool reloadOnChange = true
     ) where T : class => CreateConfiguration<T>(reloadOnChange);

    public static IConfiguration CreateConfiguration<T>(this
        IServiceCollection _,
        bool reloadOnChange = true
    ) where T : class => CreateConfiguration<T>(reloadOnChange);

    private static IConfiguration CreateConfiguration<T>(bool reloadOnChange) where T : class =>
                 _config ??= new ConfigurationBuilder()
                    .AddJsonFile($"{HostAssy.HostName<T>()}.appsettings.json", optional: false, reloadOnChange)
                    .AddJsonFile($"{HostAssy.HostName<T>()}.appsettings.{HostAssy.HostConfigName<T>()}.json", optional: true, reloadOnChange)
                    .AddEnvironmentVariables()
                    .Build();
}

public static class CreateLoggerFactory
{
    private static Logger? _logger;

    private static readonly ConfigurationReaderOptions options = new(typeof(ConsoleLoggerConfigurationExtensions).Assembly, typeof(Logger).Assembly);

    [RequiresUnreferencedCode("")]
    public static ILogger Logger<T>(this
        IHostBuilder _,
        IConfiguration configuration) where T : class => configuration.Logger<T>();

    [RequiresUnreferencedCode("")]
    public static ILogger Logger<T>(this
        IServiceCollection _,
        IConfiguration configuration) where T : class => configuration.Logger<T>();

    [RequiresUnreferencedCode("")]
    public static ILogger Logger<T>(this IConfiguration configuration) where T : class =>
        _logger ??= new LoggerConfiguration()
            .ReadFrom.Configuration(configuration, options)
            .WriteTo.Console()
            .WriteTo.File(HostAssy.LogPath<T>(configuration), shared: true)
            .Enrich.FromLogContext()
            .CreateLogger();
}