using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DSharpPlus;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders
{
    public sealed class CommandSlashMetadataBuilder : ISlashMetadataBuilder
    {
        public bool IsSubcommand { get; set; }
        public ulong? GuildId { get; set; }
        public Permissions? RequiredPermissions { get; set; }
        public Dictionary<CultureInfo, string> LocalizedNames { get; set; } = new();
        public Dictionary<CultureInfo, string> LocalizedDescriptions { get; set; } = new();

        public CommandSlashMetadataBuilder(bool isSubcommand) => IsSubcommand = isSubcommand;

        public void Verify() { }
        public bool TryVerify() => true;
        public bool TryVerify([NotNullWhen(false)] out Exception? error)
        {
            error = null;
            return true;
        }
    }
}
