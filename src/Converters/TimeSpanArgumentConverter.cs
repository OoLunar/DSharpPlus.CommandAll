using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed partial class TimeSpanArgumentConverter : IArgumentConverter<TimeSpan>
    {
        public ApplicationCommandOptionType Type { get; init; } = ApplicationCommandOptionType.String;

        public Task<Optional<TimeSpan>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        {
            if (value == "0")
            {
                return Task.FromResult(Optional.FromValue(TimeSpan.Zero));
            }
            else if (int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
            {
                return Task.FromResult(Optional.FromNoValue<TimeSpan>());
            }
            else if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out TimeSpan result))
            {
                return Task.FromResult(Optional.FromValue(result));
            }
            else
            {
                Match m = TimeSpanParseRegex().Match(value);
                int ds = m.Groups["days"].Success ? int.Parse(m.Groups["days"].Value) : 0;
                int hs = m.Groups["hours"].Success ? int.Parse(m.Groups["hours"].Value) : 0;
                int ms = m.Groups["minutes"].Success ? int.Parse(m.Groups["minutes"].Value) : 0;
                int ss = m.Groups["seconds"].Success ? int.Parse(m.Groups["seconds"].Value) : 0;

                result = TimeSpan.FromSeconds((ds * 24 * 60 * 60) + (hs * 60 * 60) + (ms * 60) + ss);
                return result.TotalSeconds < 1
                    ? Task.FromResult(Optional.FromNoValue<TimeSpan>())
                    : Task.FromResult(Optional.FromValue(result));
            }
        }

        [GeneratedRegex("^((?<days>\\d+)d\\s*)?((?<hours>\\d+)h\\s*)?((?<minutes>\\d+)m\\s*)?((?<seconds>\\d+)s\\s*)?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.CultureInvariant)]
        private static partial Regex TimeSpanParseRegex();
    }
}
