using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace $safeprojectname$.Commands.Info;

internal class InfoCommand(
    ILogger logger,
    IConfiguration config
    ) : BaseCommand(logger, config), IInfo
{
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
        try
        {
            return true;
        }
        catch { }
        return false;
    }
}