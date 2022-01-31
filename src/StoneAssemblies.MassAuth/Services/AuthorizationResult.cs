// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationResult.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services
{
    using System.Collections.Generic;

    /// <summary>
    /// The authorization result.
    /// </summary>
    public class AuthorizationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationResult"/> class.
        /// </summary>
        /// <param name="isAuthorized">
        /// The is authorized.
        /// </param>
        /// <param name="forbiddanceReason">
        /// The forbiddance reason.
        /// </param>
        private AuthorizationResult(bool isAuthorized, Dictionary<string, object> forbiddanceReason = null)
        {
            this.IsAuthorized = isAuthorized;
            this.ForbiddanceReason = forbiddanceReason;
        }

        /// <summary>
        /// Gets the forbiddance reason.
        /// </summary>
        public Dictionary<string, object> ForbiddanceReason { get; }

        /// <summary>
        /// Gets a value indicating whether is authorized.
        /// </summary>
        public bool IsAuthorized { get; }

        /// <summary>
        /// The authorized.
        /// </summary>
        /// <returns>
        /// The <see cref="AuthorizationResult"/>.
        /// </returns>
        public static AuthorizationResult Authorized()
        {
            return new AuthorizationResult(true);
        }

        /// <summary>
        /// The forbidden.
        /// </summary>
        /// <param name="forbiddanceReason">
        /// The forbiddance reason.
        /// </param>
        /// <returns>
        /// The <see cref="AuthorizationResult"/>.
        /// </returns>
        public static AuthorizationResult Forbidden(Dictionary<string, object> forbiddanceReason = null)
        {
            return new AuthorizationResult(false, forbiddanceReason);
        }
    }
}