using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Arguments
{
    public interface IArgumentConverter
    {
        static abstract ApplicationCommandOptionType Type { get; }
        Task<IOptional> ConvertAsync(CommandContext context, CommandParameter parameter, string value);
    }
}
