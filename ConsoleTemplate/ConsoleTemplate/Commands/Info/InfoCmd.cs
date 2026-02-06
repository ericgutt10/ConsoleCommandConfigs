using McMaster.Extensions.CommandLineUtils;

namespace $safeprojectname$.Commands.Info;

[Command(Name = "Info", Description = "General Command Information")]
internal class InfoCmd(IProgram program) : BaseCmd(program)
{

    protected async override Task<int> OnExecuteAsync(CommandLineApplication app)
    {
        return await Program.InfoCmd!.InvokeAsync(app);
    }
}