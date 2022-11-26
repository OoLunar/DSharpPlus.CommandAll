using System.Collections.Generic;
using System.Globalization;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders
{
    public interface ISlashMetadataBuilder : IBuilder
    {
        Dictionary<CultureInfo, string> LocalizedNames { get; set; }
        Dictionary<CultureInfo, string> LocalizedDescriptions { get; set; }
    }
}
