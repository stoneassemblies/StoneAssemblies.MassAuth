// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeByRuleAttribute.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services.Attributes
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The authorize by rule.
    /// </summary>
    public class AuthorizeByRuleAttribute : ServiceFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeByRuleAttribute"/> class.
        /// </summary>
        public AuthorizeByRuleAttribute()
            : base(typeof(AuthorizeByRuleFilter))
        {
        }
    }
}
