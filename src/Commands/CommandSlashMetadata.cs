using System.Collections.Generic;
using System.Globalization;
using DSharpPlus;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed class CommandSlashMetadata
    {
        public bool IsSubcommand { get; set; }
        public ulong? GuildId { get; set; }
        public Permissions? RequiredPermissions { get; set; }
        public Dictionary<CultureInfo, string> LocalizedNames { get; set; } = new();
        public Dictionary<CultureInfo, string> LocalizedDescriptions { get; set; } = new();

        public CommandSlashMetadata(CommandSlashMetadataBuilder builder)
        {
            builder.Verify();
            IsSubcommand = builder.IsSubcommand;
            GuildId = builder.GuildId;
            RequiredPermissions = builder.RequiredPermissions;
            LocalizedNames = builder.LocalizedNames;
            LocalizedDescriptions = builder.LocalizedDescriptions;
        }
    }
}
