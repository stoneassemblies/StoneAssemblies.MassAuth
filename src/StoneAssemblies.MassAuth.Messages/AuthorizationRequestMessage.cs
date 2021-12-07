// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationRequestMessage.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Messages
{
    using System.Collections.Generic;
    using System.Security.Claims;

    using StoneAssemblies.MassAuth.Messages.Interfaces;

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

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken { get; set; }
    }
}