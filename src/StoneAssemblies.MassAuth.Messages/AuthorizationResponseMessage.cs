// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationResponseMessage.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
