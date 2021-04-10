namespace StoneAssemblies.MassAuth.Messages
{
    using System.Collections.Generic;
    using System.Security.Claims;

    /// <summary>
    /// The authorization request message.
    /// </summary>
    public class AuthorizationRequestMessage : IAuthorizationRequestMessage
    {
        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        public IEnumerable<Claim> Claims { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }
    }
}