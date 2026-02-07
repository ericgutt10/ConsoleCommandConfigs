using ConsoleTemplate;
using McMaster.Extensions.CommandLineUtils;

namespace ConsoleTemplate.Commands;

[HelpOption("-?|-h|--help")]
internal abstract class BaseCmd(IProgram program)
{


    [Option(Description = "Set to an existing folder in the config json 'AppSettings:LogsDirectory' section; Otherwise defualts to the '%temp%\\<program_name>\\' folder", ShortName = "l")]
    public string? LogsDirectory { get; set; }

    public IProgram Program { get; } = program;

    protected abstract Task<int> OnExecuteAsync(CommandLineApplication app);

}