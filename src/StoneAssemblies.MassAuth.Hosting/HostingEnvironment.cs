// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostingEnvironment.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting
{
    using System;

    /// <summary>
    /// The hosting environment.
    /// </summary>
    public static class HostingEnvironment
    {
        /// <summary>
        /// The is marathon hosted.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsMarathonHosted()
        {
            return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MARATHON_APP_ID"));
        }
    }
}