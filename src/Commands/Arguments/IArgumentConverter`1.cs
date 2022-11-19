using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Arguments
{
    public interface IArgumentConverter<T> : IArgumentConverter
    {
        new Task<Optional<T>> ConvertAsync(CommandContext context, CommandParameter parameter, string value);
        Task<IOptional> IArgumentConverter.ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult<IOptional>(ConvertAsync(context, parameter, value).GetAwaiter().GetResult());
    }
}
