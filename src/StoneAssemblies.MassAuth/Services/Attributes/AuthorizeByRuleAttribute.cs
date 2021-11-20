// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeByRuleAttribute.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services.Attributes
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;

    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    ///     The authorize by rule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeByRuleAttribute : ServiceFilterAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthorizeByRuleAttribute" /> class.
        /// </summary>
        /// <param name="messageTypes">
        ///     Message types.
        /// </param>
        public AuthorizeByRuleAttribute(params Type[] messageTypes)
            : base(typeof(AuthorizeByRuleFilter))
        {
            this.MessageTypes = messageTypes?.Where(type => type != null && typeof(MessageBase).IsAssignableFrom(type)).ToArray();
        }

        /// <summary>
        ///     Gets the message types.
        /// </summary>
        public Type[] MessageTypes { get; }
    }
}