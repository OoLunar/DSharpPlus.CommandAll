using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.CommandAll.Managers;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    public sealed class NullableArgumentConverter<T> : IArgumentConverter<T> where T : class
    {
        public ApplicationCommandOptionType OptionType => _innerConverter.OptionType;
        public ArgumentParsingBehavior ParsingBehavior => _innerConverter.ParsingBehavior;
        private readonly IArgumentConverter<T> _innerConverter;

        [SuppressMessage("Style", "IDE0045:Convert to conditional expression", Justification = "This is more readable")]
        public NullableArgumentConverter(ArgumentConverterManager manager, IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            else if (!manager.TryGetConverter(typeof(T), out ArgumentConverterDefinition? converter))
            {
                throw new ArgumentException($"No converter found for type {typeof(T)}");
            }
            else
            {
                _innerConverter = (IArgumentConverter<T>)converter.GetOrCreateConverter(serviceProvider);
            }
        }

        public bool CanConvert(Type type) => _innerConverter.CanConvert(Nullable.GetUnderlyingType(type) ?? type);

        public Task<Optional<T>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null)
        {
            if (context.InvocationType != CommandInvocationType.SlashCommand)
            {
                return Task.FromResult(Optional.FromNoValue<T>());
            }
            else if (parameter is null)
            {
                throw new InvalidOperationException($"{nameof(ParsingBehavior)} requires the parameter to not be null!");
            }

            // We're going to assume that we're handling a top level command
            IEnumerable<DiscordInteractionDataOption> choices = context.Interaction!.Data.Options.First().Options;
            DiscordInteractionDataOption? choice = choices.FirstOrDefault(choice => choice.Name == parameter.SlashNames[0]);
            return choice is null ? Task.FromResult(Optional.FromValue<T>(null!)) : _innerConverter.ConvertAsync(context, value, parameter);
        }
    }
}
