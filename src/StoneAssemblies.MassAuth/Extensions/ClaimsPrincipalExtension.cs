// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClaimsPrincipalExtension.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Extensions
{
    using System.Linq;
    using System.Security.Claims;

    /// <summary>
    /// The claims principal extension.
    /// </summary>
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// Gets the user id.
        /// </summary>
        /// <param name="this">
        /// The this.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetUserId(this ClaimsPrincipal @this)
        {
            var preferredUserNameClaim = @this.Claims.FirstOrDefault(claim => claim.Type.Equals("preferred_username"));
            var userId = preferredUserNameClaim?.Value ?? @this.Identity?.Name;
            return userId;
        }
    }
}
