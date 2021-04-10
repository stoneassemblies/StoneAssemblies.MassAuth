namespace StoneAssemblies.MassAuth.Messages
{
    /// <summary>
    /// The AuthorizationRequestMessage interface.
    /// </summary>
    /// <typeparam name="TPayload">
    /// The payload type.
    /// </typeparam>
    public interface IAuthorizationRequestMessage<out TPayload> : IAuthorizationRequestMessage
        where TPayload : MessageBase
    {
        /// <summary>
        /// Gets the payload.
        /// </summary>
        TPayload Payload { get; }
    }
}