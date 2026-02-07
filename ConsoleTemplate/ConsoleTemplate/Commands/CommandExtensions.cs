using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;

namespace ConsoleTemplate.Commands;

public static class CommandExtensions
{
    public static string AggregateChangeList(this
            List<(string prop, string? value)> changed,
            string leadString = "Validate: "
        )
    {
        StringBuilder sb = new($"{Environment.NewLine}{leadString}");
        if (changed.Count > 0)
        {
            sb.Append(changed.Aggregate($"{Environment.NewLine}", (s, i) => s == "" ? $"{new(' ', 4)}{i.prop} - '{i.value}',{Environment.NewLine}" : $"{s}{new(' ', 4)}{i.prop} - '{i.value}',{Environment.NewLine}"));
        }
        return sb.ToString();
    }

    public static string? GetHasNoValue(this CommandOption o)
    {
        return o.HasValue() ? "true" : null;
    }

    public static string? GetSingleOrNoValue(this CommandOption o)
    {
        string? result = default!;
        if (o.HasValue())
        {
            result = o.Values?.Any() switch
            {
                true => o.Values[0],
                _ => null
            };
        }
        return result;
    }

    public static string? GetConfig(this string? value, IConfiguration? config, string? configKey)
    {
        if (value is not null) { return value; }

        if (string.IsNullOrWhiteSpace(configKey)) { return null; }

        _ = config.ValidateConfigKey(configKey) ? true :
            throw new Exception("",
                new KeyNotFoundException($"Config key not found: {configKey}"));

        string? result = config?[configKey] switch
        {
            string v when !string.IsNullOrWhiteSpace(v) => v,
            _ => default
        };

        return result;
    }

    public static bool ValidateConfigKey(this
        IConfiguration? config, string? configKey)
    {
        if (config is null) { return false; }
        if (string.IsNullOrWhiteSpace(configKey)) { return false; }

        var keys = configKey.Split(":");
        var parentKey = keys.Length switch
        {
            0 => throw new Exception("inconceivable"),
            1 => configKey,
            _ => string.Join(":", keys.SkipLast(1))
        };
        var sct = config.GetSection(parentKey);

        var valid = sct?
                .GetChildren()
                .Select(p => p.Path)
                .Any(p => p.Contains(keys.Last())) ?? false;

        return valid;
    }

    public static CommandOption? GetOption(this
        IEnumerable<CommandOption> commandOptions,
        string? optionName, bool throwOnNotFound = true)
    {
        var result = commandOptions.FirstOrDefault(o =>
        {
            try
            {
                ArgumentNullException.ThrowIfNull(o.LongName);

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
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine(ex.Message);
                if (throwOnNotFound)
                {
                    throw new Exception("", ex);
                }
            }

            return false;
        });

        return result is not null ? result :
            throw new Exception($"CommandOption not found '{optionName}'");
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

    public static bool ValidateBoolOption(this string? value, bool? defaultValue = true)
    {
        bool result = bool.TryParse(value, out result) ? result : defaultValue ?? false;
        return result;
    }

    //public static RELEASE_OP ValidateReleaseOpOption(this string? value, RELEASE_OP? defaultValue = RELEASE_OP.NONE)
    //{
    //    RELEASE_OP result = Enum.TryParse<RELEASE_OP>(value, out var test) ? test : defaultValue ?? 0;
    //    return result;
    //}

    public static string? ValidateString(this
        string? stringValue, bool acceptEmptyAsNull = false, bool ignoreExceptions = false)
    {
        try
        {
            if (acceptEmptyAsNull && string.IsNullOrWhiteSpace(stringValue))
            {
                return null;
            }

            ArgumentNullException.ThrowIfNull(stringValue, nameof(stringValue));

            return stringValue;
        }
        catch (ArgumentException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }

        return stringValue;
    }

    /// <summary>
    /// TODO: make more generic....
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stringValue"></param>
    /// <param name="separators"></param>
    /// <param name="ignoreExceptions"></param>
    /// <returns></returns>
    public static IList<T>? ValidateList<T>(this
        string? stringValue, string[]? separators, bool ignoreExceptions = false)
    //where T: class
    {
        separators ??= ["|"];
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stringValue, nameof(stringValue));

            var strAry = stringValue?.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return [.. strAry?.OfType<T>()];
        }
        catch (ArgumentNullException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }

        return null;
    }

    public static string[]? ValidateStringArray(this
    string? stringValue, string[]? separators, bool ignoreExceptions = false)
    {
        separators ??= ["|"];
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stringValue, nameof(stringValue));

            var strAry = stringValue?.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return strAry;
        }
        catch (ArgumentNullException ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            if (!ignoreExceptions)
            {
                throw;
            }
        }

        return null;
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

    public static DateOnly? ValidateDateOnlyValue(this
    string? stringValue, bool ignoreExceptions = false, bool returnNull = false)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stringValue, nameof(stringValue));

            DateOnly value = DateOnly.TryParse(stringValue, out value) ? value :
                throw new InvalidCastException();

            return value;
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

        return returnNull ? null : DateOnly.FromDateTime(DateTime.Now);
    }

    public static TimeOnly ValidateTimeOnlyValue(this
        string? stringValue, bool ignoreExceptions = false)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stringValue, nameof(stringValue));

            TimeOnly value = TimeOnly.TryParse(stringValue, out value) ? value :
                throw new InvalidCastException();

            return value;
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

        return TimeOnly.MinValue;
    }

    public static DirectoryInfo? ValidateDirectory(this
        string? path, bool createIfNotFound = false,
        bool ignoreExceptions = false)
    {
        if (createIfNotFound)
        {
            path ??= AppContext.BaseDirectory;
        }

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
                    throw new DirectoryNotFoundException(
                        $"{nameof(DirectoryNotFoundException)} {path}");
                }
            }
            return di;
        }
        catch (ArgumentException ex)
        {
            Debug.WriteLine(ex.Message, ex);
            if (ignoreExceptions)
            {
                return di;
            }

            throw new Exception("", ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            Debug.WriteLine(ex.Message);
            if (ignoreExceptions)
            {
                return di;
            }

            throw new Exception("", ex);
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
            ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(ValidateFileName));

            fi = new FileInfo(fileName);

            if (fi.Exists || fi.Directory.Exists) { return fi; }

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

    public static string? ValidateUri(this
        string? uriStr,
        bool ignoreExceptions = false,
        string? src = null)
    {
        try
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(uriStr, nameof(uriStr));

            string? uri = Uri.TryCreate(uriStr,
                new UriCreationOptions() { DangerousDisablePathAndQueryCanonicalization = true },
                out var uriUrl) ? uriUrl.ToString() : throw new UriFormatException(uriStr);

            return uri;
        }
        catch (UriFormatException ex)
        {
            Debug.WriteLine(ex.Message);
            if (ignoreExceptions)
            {
                return uriStr;
            }

            throw new Exception($"{src}{ex.Message}", ex);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            if (ignoreExceptions)
            {
                return uriStr;
            }

            throw new Exception($"{src}{ex.Message}", ex);
        }
    }
}