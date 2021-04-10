namespace StoneAssemblies.MassAuth.Rules.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    ///     The Rule interface.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type
    /// </typeparam>
    public interface IRule<TMessage>
    {
        /// <summary>
        ///     Gets a value indicating whether is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        int Priority { get; }

        /// <summary>
        ///     The evaluate async.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        Task<bool> EvaluateAsync(TMessage message);
    }
}