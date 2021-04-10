namespace StoneAssemblies.MassAuth.Messages
{
    /// <summary>
    ///     The authorization request message.
    /// </summary>
    /// <typeparam name="TPayload">
    ///     The payload type.
    /// </typeparam>
    public class AuthorizationRequestMessage<TPayload> : AuthorizationRequestMessage,
                                                         IAuthorizationRequestMessage<TPayload>
        where TPayload : MessageBase
    {
        /// <summary>
        ///     Gets or sets the payload.
        /// </summary>
        public TPayload Payload { get; set; }

        /// <summary>
        ///     The get payload.
        /// </summary>
        /// <returns>
        ///     The <see cref="TPayload" />.
        /// </returns>
        public TPayload GetPayload()
        {
            return this.Payload;
        }
    }
}