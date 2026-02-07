using ConsoleTemplate.Commands.Info;
using ConsoleTemplate.Lib;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

using Serilog;

namespace ConsoleTemplate.Commands;

internal abstract class BaseCommand(
    ILogger? logger,
    IConfiguration config)
{
    public ILogger? Logger { get; } = logger;
    public IConfiguration Config { get; } = config;
    public DirectoryInfo? LogsDirectory { get; private set; }

    protected virtual async Task<int> RunAsync()
    {
        return await Task.FromResult(0);
    }

    protected virtual bool Validate(CommandLineApplication app)
    {
        Logger?.Information($"Validate {nameof(BaseCommand)}");
        List<(string prop, string? value)> changed = [];

        try
        {
            LogsDirectory = app.Options
                .GetOption(nameof(BaseCmd.LogsDirectory))?
                .Value()!
                .GetConfig(Config, "AppSettings:LogsDirectory")
                .ValidateDirectory(createIfNotFound: false, ignoreExceptions: true);

            changed.Add((nameof(LogsDirectory), $"{LogsDirectory}"));

            var chgStr = changed.AggregateChangeList($"Validate {nameof(BaseCmd)}: ");

            Logger?.Information("{@TableName} {sb}", Logger.TraceSrc(), chgStr);

            return true;
        }
        catch (Exception ex)
        {
            Logger?.Warning("{@TableName} {cmd} Validation Error: {ex}{inner}",
                Logger.TraceSrc(), nameof(BaseCommand), ex.Message,
                    ex.InnerException is null ? "" : ex.InnerException.Message);
        }
        return false;
    }
}