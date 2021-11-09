// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeByRuleFilterConfigurationOptions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services.Options
{
    /// <summary>
    /// The authorize by rule filter configuration options.
    /// </summary>
    public class AuthorizeByRuleFilterConfigurationOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether include reason.
        /// </summary>
        public bool ReturnForbiddanceReason { get; set; }
    }
}