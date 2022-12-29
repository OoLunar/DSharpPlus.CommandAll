namespace DSharpPlus.CommandAll.Commands.Enums
{
    public enum ArgumentParsingBehavior
    {
        /// <summary>
        /// Result will always be the same if the same value is passed
        /// </summary>
        Static,

        /// <summary>
        /// Result changes based on the CommandParameter passed and should be calculated per argument
        /// </summary>
        RequiresCommandParameter
    }
}
