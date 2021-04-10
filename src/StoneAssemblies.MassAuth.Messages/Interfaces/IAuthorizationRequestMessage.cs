// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationRequestMessage.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Messages.Interfaces
{
    using System.Collections.Generic;
    using System.Security.Claims;

    /// <summary>
    /// The AuthorizationRequestMessage interface.
    /// </summary>
    public interface IAuthorizationRequestMessage
    {
        /// <summary>
        ///     Gets or sets the claims.
        /// </summary>
        IEnumerable<Claim> Claims { get; set; }

        /// <summary>
        ///     Gets the user id.
        /// </summary>
        string UserId { get; }
    }
}