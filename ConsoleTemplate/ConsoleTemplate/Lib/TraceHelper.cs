using Serilog;
using Serilog.Events;
using System.Runtime.CompilerServices;

namespace ConsoleTemplate.Lib;

public enum LogLevelExt
{
    Verbose = LogEventLevel.Verbose,
    Debug = LogEventLevel.Debug,
    Information = LogEventLevel.Information,
    Warning = LogEventLevel.Warning,
    Error = LogEventLevel.Error,
    Fatal = LogEventLevel.Fatal,
    Raw = LogEventLevel.Fatal + 1,
    RawVerbose = Raw + 1
}

public static class TraceHelper
{
    public static string TraceSrc(this ILogger? _,
       [CallerMemberName] string memberName = "",
       [CallerFilePath] string sourcefilePath = "",
       [CallerLineNumber()] long sourceLineNumber = 0)
    {
        return $"{Path.GetFileNameWithoutExtension(sourcefilePath)}.{memberName}.{sourceLineNumber}:";
    }

    public static void LogTrace(
               this ILogger? logger,
               LogEventLevel level,
               string message = "",
               [CallerMemberName] string memberName = "",
               [CallerFilePath] string sourcefilePath = "",
               [CallerLineNumber()] long sourceLineNumber = 0)
    {
        LogTrace(
            logger, (LogLevelExt)level, message, memberName, sourcefilePath, sourceLineNumber)
            ;
    }

    public static void LogTrace(
       this ILogger? logger,
       LogLevelExt level,
       string message = "",
       [CallerMemberName] string memberName = "",
       [CallerFilePath] string sourcefilePath = "",
       [CallerLineNumber()] long sourceLineNumber = 0)
    {
        string traceMsg;
        if (level >= LogLevelExt.Raw)
        {
            switch (level)
            {
                case LogLevelExt.Raw:
                    {
                        logger?.Information(message);
                        break;
                    }
                case LogLevelExt.RawVerbose:
                    {
                        logger?.Verbose(message);
                        break;
                    }
            }
        }
        else
        {
            traceMsg = $"{Path.GetFileNameWithoutExtension(sourcefilePath)}.{memberName}.{sourceLineNumber}:{message}";

            switch (level)
            {
                //0
                case LogLevelExt.Verbose:
                    {
                        logger?.Verbose(traceMsg);
                        break;
                    }
                //1
                case LogLevelExt.Debug:
                    {
                        logger?.Debug(traceMsg);
                        break;
                    }
                //2
                case LogLevelExt.Information:
                    {
                        logger?.Information(traceMsg);
                        break;
                    }
                //3
                case LogLevelExt.Warning:
                    {
                        logger?.Warning(traceMsg);
                        break;
                    }
                //4
                case LogLevelExt.Error:
                    {
                        logger?.Error(traceMsg);
                        break;
                    }
                //5
                case LogLevelExt.Fatal:
                    {
                        logger?.Fatal(traceMsg);
                        break;
                    }
                //2
                default:
                    {
                        logger?.Information(traceMsg);
                        break;
                    }
            }
        }
    }
}