using McMaster.Extensions.CommandLineUtils;

namespace ConsoleTemplate;

public interface IInvoke
{
    Task<int> InvokeAsync(CommandLineApplication app);
}

public interface IInfo : IInvoke
{ }

public interface IProgram
{
    public IInfo? InfoCmd { get; }
}