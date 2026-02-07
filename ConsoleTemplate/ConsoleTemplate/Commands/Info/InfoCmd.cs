using McMaster.Extensions.CommandLineUtils;

namespace ConsoleTemplate.Commands.Info;

[Command(Name = "Info", Description = "General Command Information")]
internal class InfoCmd(IProgram program) : BaseCmd(program)
{
    [Option(Description = @"Target root directory to use for the command; Default '.\' (CWD)", ShortName = "id")]
    public string? InputDirectory { get; set; }

    [Option(Description = @"Output directory Name used for performing commands; Default '.\' (CWD)", ShortName = "od")]
    public string? OutputDirectory { get; set; }

    protected override async Task<int> OnExecuteAsync(CommandLineApplication app)
    {
        return await Program.InfoCmd!.InvokeAsync(app);
    }
}