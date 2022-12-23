namespace DSharpPlus.CommandAll.Commands.Enums
{
    /// <summary>
    /// The naming strategy to be used when registering a command parameter as a slash command.
    /// </summary>
    public enum CommandParameterNamingStrategy
    {
        /// <summary>
        /// random_parameter_name_1
        /// </summary>
        SnakeCase,

        /// <summary>
        /// random-parameter-name-1
        /// </summary>
        KebabCase,

        /// <summary>
        /// randomparametername1
        /// </summary>
        LowerCase
    }
}
