// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionExecutingContextExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;

    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Services.Attributes;

    /// <summary>
    ///     The action executing context extensions.
    /// </summary>
    public static class ActionExecutingContextExtensions
    {
        /// <summary>
        ///     Get messages.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="List{MessageBase}" />.
        /// </returns>
        public static List<MessageBase> GetMessages(this ActionExecutingContext context)
        {
            var messages = context.ActionArguments.Values.OfType<MessageBase>().ToList();
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var authorizeByRuleAttribute = controllerActionDescriptor.MethodInfo.GetCustomAttribute<AuthorizeByRuleAttribute>();
                if (authorizeByRuleAttribute?.MessageTypes != null)
                {
                    foreach (var messageType in authorizeByRuleAttribute.MessageTypes)
                    {
                        var message = Activator.CreateInstance(messageType) as MessageBase;
                        context.Fill(message);
                        messages.Add(message);
                    }
                }
            }

            return messages;
        }

        /// <summary>
        ///     Fill a message from the context.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        private static void Fill(this ActionExecutingContext context, MessageBase message)
        {
            foreach (var actionArgument in context.ActionArguments)
            {
                var actionArgumentValue = actionArgument.Value;
                if (actionArgumentValue != null)
                {
                    var propertyInfo = message.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(
                        p => p.Name.Equals(actionArgument.Key, StringComparison.InvariantCultureIgnoreCase)
                             && actionArgumentValue.GetType().IsAssignableFrom(p.PropertyType));

                    propertyInfo?.SetValue(message, actionArgumentValue);
                }
            }
        }
    }
}