using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

using Serilog;

namespace $safeprojectname$.Commands;

internal abstract class BaseCommand(
    ILogger? logger,
    IConfiguration config)
{
    public ILogger? Logger { get; } = logger;
    public IConfiguration Config { get; } = config;
    public string? LogsDirectory { get; private set; }

    protected virtual async Task<int> RunAsync()
    {
        return await Task.FromResult(0);
    }

    protected virtual bool Validate(CommandLineApplication app)
    {
        Logger?.Information($"Validate BaseCommand");

        try
        {
            LogsDirectory = app.Options
                .GetOption(nameof(BaseCmd.LogsDirectory))?
                .Value()!
                .GetConfig(Config, "AppSettings:LogsDirectory")
                .ValidateDirectory(createIfNotFound: false, ignoreExceptions: true);

            Logger?.Information($"AppSettings:LogsDirectory - {LogsDirectory}");

            return true;
        }
        catch (Exception ex)
        {
            Logger?.Error(ex.Message);
        }
        return false;
    }
}