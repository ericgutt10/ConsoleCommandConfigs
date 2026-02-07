using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Console.Commands;

public static class CommandExtensions
{
    public static string? GetHasNoValue(this CommandOption o)
    {
        return o.HasValue() ? "true" : null;
    }

    public static string? GetSingleOrNoValue(this CommandOption o)
    {
        string? result = default!;
        if (o.HasValue())
        {
            result = o.Values?.FirstOrDefault() switch
            {
                string v => v,
                _ => null
            };
        }
        return result;
    }

    public static string? GetConfig(this string? value, IConfiguration config, string? configName)
    {
        if (value is not null) { return value; }

        if (string.IsNullOrWhiteSpace(configName)) { return null; }

        var result = config.GetValue<string>(configName);

        return result;
    }

    public static CommandOption? GetOption(this
        IEnumerable<CommandOption> commandOptions,
        string? optionName, bool throwOnNotFound = false)
    {
        return commandOptions.FirstOrDefault(o =>
        {
            _ = string.IsNullOrWhiteSpace(o.LongName) ?
                throw new ArgumentNullException(nameof(o.LongName)) : true;

            bool found = string.Compare(
                optionName,
                o.LongName.Replace("-", "").Replace("_", ""),
                StringComparison.OrdinalIgnoreCase) == 0;

            if (found)
            {
                found = o.OptionType switch
                {
                    CommandOptionType.NoValue => true,
                    CommandOptionType.SingleValue => true,
                    CommandOptionType.SingleOrNoValue => true,
                    _ => o.HasValue() || (throwOnNotFound ? throw new ArgumentException($"{o.LongName} has no value") : true)
                };
            }

            return found;
        });
    }

    public static int? ValidateIntValue(this
        string? stringValue, bool ignoreExceptions = false)
    {
        int? value = default!;

        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stringValue);

            value = int.TryParse(stringValue, out int val) ? val :
               throw new InvalidCastException();
        }
        catch (ArgumentException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }
        catch (InvalidCastException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }

        return value;
    }
    public static long? ValidateLongValue(this
    string? stringValue, bool ignoreExceptions = false)
    {
        long? value = default!;

        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stringValue);

            value = long.TryParse(stringValue, out long val) ? val :
               throw new InvalidCastException();
        }
        catch (ArgumentException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }
        catch (InvalidCastException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }

        return value;
    }
    public static decimal? ValidateDecimalValue(this
        string? stringValue, bool ignoreExceptions = false)
    {
        decimal? value = default!;

        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stringValue);

            value = decimal.TryParse(stringValue, out decimal val) ? val :
                throw new InvalidCastException();
        }
        catch (ArgumentException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }
        catch (InvalidCastException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }

        return value;
    }

    public static bool ValidateNoValueOption(this CommandOption? option)
    {
        bool result = option is not null && option!.HasValue();
        return result;
    }

    public static bool ValidateBoolOption(this string? value)
    {
        bool result = bool.TryParse(value, out result) && result;
        return result;
    }

    public static string? ValidateString(this
        string? stringValue, bool acceptEmptyString = false, bool ignoreExceptions = false)
    {
        try
        {
            _ = stringValue is null ? throw new ArgumentNullException(nameof(stringValue)) : true;

            _ = !string.IsNullOrWhiteSpace(stringValue) || (acceptEmptyString ? true :
              throw new ArgumentException("Empty string or whitespace found", nameof(stringValue)));

            return stringValue;
        }
        catch (ArgumentNullException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }
        return string.Empty;
    }

    public static DateTime ValidateDateTimeValue(this
        string stringValue, bool ignoreExceptions = false)
    {
        try
        {
            _ = string.IsNullOrWhiteSpace(stringValue) ?
                throw new ArgumentNullException(nameof(stringValue)) : true;

            DateTime value = DateTime.TryParse(stringValue, out value) ? value :
                throw new InvalidCastException();

            return value;
        }
        catch (ArgumentNullException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }
        catch (InvalidCastException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }
        return DateTime.Now;
    }

    public static string? ValidateDirectory(this
        string? path, bool createIfNotFound = true,
        bool ignoreExceptions = false)
    {
        path ??= AppContext.BaseDirectory;

        DirectoryInfo? di = default;
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                if (createIfNotFound)
                {
                    di.Create();
                }
                else
                {
                    throw new DirectoryNotFoundException(path);
                }
            }
            return di.FullName;
        }
        catch (ArgumentException ex)
        {
            Debug.WriteLine(ex.Message, ex);
            if (ignoreExceptions)
            {
                return di?.FullName;
            }

            throw;
        }
        catch (DirectoryNotFoundException ex)
        {
            Debug.WriteLine(ex.Message);
            if (ignoreExceptions)
            {
                return di?.FullName;
            }

            throw;
        }
    }

    public static FileInfo? ValidateFileName(this
        string? fileName,
        string? directory = null,
        bool ignoreExceptions = false)
    {
        FileInfo? fi = default;
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

            directory ??= AppContext.BaseDirectory;

            fi = new FileInfo(Path.Combine(directory, fileName ?? ""));

            _ = fi.Exists ? true :
                throw new FileNotFoundException(fileName);

            return fi;
        }
        catch (ArgumentException ex)
        {
            Debug.WriteLine(ex.Message, ex);
            if (ignoreExceptions)
            {
                return fi;
            }

            throw;
        }
        catch (FileNotFoundException ex)
        {
            Debug.WriteLine(ex.Message);
            if (ignoreExceptions)
            {
                return fi;
            }

            throw;
        }
    }
}