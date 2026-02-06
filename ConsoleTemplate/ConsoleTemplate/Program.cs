using $safeprojectname$.Commands.Info;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Runtime;

namespace $safeprojectname$;

[Command(Name = "Template", Description = "Console Template"),
    Subcommand(typeof(InfoCmd))
]
internal class Program : IProgram
{
    static Program()
    {
        HostBuilder = new HostBuilder()
           .ConfigureServices(static (ctx, services) =>
           {
               services.AddSingleton(services.CreateConfiguration<Program>());
               services.AddSingleton(services.Logger<Program>(services.CreateConfiguration<Program>()));
               services.AddScoped<IProgram, Program>();
               services.AddScoped<IInfo, InfoCommand>();
           });
    }

    public IInfo? InfoCmd { get; }
    public IConfiguration Configuration { get; }
    public ILogger Logger { get; }

    private static async Task<int>? Main(string[] args)
        => await HostBuilder!.RunCommandLineApplicationAsync<Program>(args, (app) =>
        {

            app.OnExecuteAsync(async (CancellationToken) =>
            {
                app.ShowHelp(true);
                app.ShowRootCommandFullNameAndVersion();
                return await Task.FromResult(0);
            });
        });

    public static IHostBuilder? HostBuilder { get; }

    public Program(
            IConfiguration configuration,
            ILogger logger,
            IInfo infoCmd
    )
    {
        InfoCmd = infoCmd;
        Configuration = configuration;
        Logger = logger;

        Logger.Information($"LogPath - {HostAssy.LogPath<Program>(Configuration)}");
    }
}