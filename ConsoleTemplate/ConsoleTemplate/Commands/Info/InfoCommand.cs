using ConsoleTemplate.Lib;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Threading.Channels;

namespace ConsoleTemplate.Commands.Info;

internal class InfoCommand(
    ILogger logger,
    IConfiguration config
    ) : BaseCommand(logger, config), IInfo
{

    public DirectoryInfo? InputDirectory { get; private set; }
    public DirectoryInfo? OutputDirectory { get; private set; }

    public async Task<int> InvokeAsync(CommandLineApplication app)
    {
        if (Validate(app))
        {
            return await RunAsync();
        }
        return await Task.FromResult(0);
    }

    protected override Task<int> RunAsync()
    {
        return base.RunAsync();
    }

    protected override bool Validate(CommandLineApplication app)
    {
        if (!base.Validate(app)) { return false; }

        Logger?.Information($"Validate {nameof(InfoCommand)}");

        List<(string prop, string? value)> changed = [];
        string cmdNameBase = $"{AppConfig.COMMANDS}:{nameof(InfoCmd)}";

        try
        {
            InputDirectory = app.Options
                .GetOption(nameof(InfoCmd.InputDirectory))?
                .Value()
                .GetConfig(Config, $"{cmdNameBase}:{nameof(InputDirectory)}")
                .ValidateDirectory(ignoreExceptions: true);

            changed.Add((nameof(InputDirectory), $"{InputDirectory}"));

            OutputDirectory = app.Options
                .GetOption(nameof(InfoCmd.OutputDirectory))?
                .Value()
                .GetConfig(Config, $"{cmdNameBase}:{nameof(OutputDirectory)}")
                .ValidateDirectory(ignoreExceptions: true);

            changed.Add((nameof(OutputDirectory), $"{OutputDirectory}"));

            var chgStr = changed.AggregateChangeList($"Validate {nameof(InfoCmd)}: ");

            Logger?.Information("{@TableName} {sb}", Logger.TraceSrc(), chgStr);

            return true;
        }
        catch (Exception ex)
        {
            Logger?.Warning("{@TableName} {cmd} Validation Error: {ex}{inner}",
                Logger.TraceSrc(), nameof(InfoCommand), ex.Message,
                    ex.InnerException is null ? "" : ex.InnerException.Message);
        }
        return false;
    }
}