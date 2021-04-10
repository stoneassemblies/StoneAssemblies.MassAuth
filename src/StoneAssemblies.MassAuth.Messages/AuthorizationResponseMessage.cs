namespace StoneAssemblies.MassAuth.Messages
{
    /// <summary>
    ///     The authorization response message.
    /// </summary>
    public class AuthorizationResponseMessage
    {
        /// <summary>
        ///     Gets or sets a value indicating whether is authorized.
        /// </summary>
        public bool IsAuthorized { get; set; }

        /// <summary>
        ///     Gets or sets the reason.
        /// </summary>
        public string Reason { get; set; }
    }
}