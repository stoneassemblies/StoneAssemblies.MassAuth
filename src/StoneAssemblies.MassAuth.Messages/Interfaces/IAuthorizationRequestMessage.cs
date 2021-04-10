namespace StoneAssemblies.MassAuth.Messages
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public interface IAuthorizationRequestMessage
    {
        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        IEnumerable<Claim> Claims { get; set; }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        string UserId { get; }
    }
}