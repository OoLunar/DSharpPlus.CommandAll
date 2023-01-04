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
    public sealed class NullableArgumentConverter<T> : ArgumentConverter<T?> where T : class
    {
        public override ApplicationCommandOptionType OptionType => _innerConverter.OptionType;
        public override ArgumentParsingBehavior ParsingBehavior => _innerConverter.ParsingBehavior;
        private readonly ArgumentConverter<T> _innerConverter;

        [SuppressMessage("Style", "IDE0045:Convert to conditional expression", Justification = "This is more readable")]
        public NullableArgumentConverter(IArgumentConverterManager manager, IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            IReadOnlyList<ArgumentConverterDefinition> converters = manager.GetConverters(Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
            if (!converters.Any())
            {
                throw new ArgumentException($"No converter found for type {typeof(T)}");
            }
            else
            {
                _innerConverter = (ArgumentConverter<T>)converters[0].GetOrCreateConverter(serviceProvider);
            }
        }

        public override bool CanConvert(Type type) => _innerConverter.CanConvert(Nullable.GetUnderlyingType(type) ?? type);

        public override async Task<Optional<T?>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null)
        {
            if (context.InvocationType != CommandInvocationType.SlashCommand)
            {
                return Optional.FromNoValue<T?>();
            }
            else if (parameter is null)
            {
                throw new InvalidOperationException($"{nameof(ParsingBehavior)} requires the parameter to not be null!");
            }

            // We're going to assume that we're handling a top level command
            IEnumerable<DiscordInteractionDataOption> choices = context.Interaction!.Data.Options.First().Options;
            DiscordInteractionDataOption? choice = choices.FirstOrDefault(choice => choice.Name == parameter.SlashNames[0]);

            if (choice is null)
            {
                return Optional.FromNoValue<T?>();
            }
            else
            {
                Optional<T> inner = await _innerConverter.ConvertAsync(context, value, parameter).ConfigureAwait(false);
                return inner.HasValue ? Optional.FromValue<T?>(inner.Value) : Optional.FromNoValue<T?>();
            }
        }
    }
}
