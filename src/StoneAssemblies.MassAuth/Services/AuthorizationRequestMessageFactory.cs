// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationRequestMessageFactory.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    /// The authorization request message factory.
    /// </summary>
    public static class AuthorizationRequestMessageFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="AuthorizationRequestMessage"/>.
        /// </summary>
        /// <param name="payload">
        /// The payload.
        /// </param>
        /// <returns>
        /// The <see cref="AuthorizationRequestMessage"/>.
        /// </returns>
        public static AuthorizationRequestMessage From(MessageBase payload)
        {
            var makeGenericType = typeof(AuthorizationRequestMessage<>).MakeGenericType(payload.GetType());
            var authorizationMessage = Activator.CreateInstance(makeGenericType);
            var payloadPropertyName = nameof(AuthorizationRequestMessage<MessageBase>.Payload);
            var propertyInfo = makeGenericType.GetProperty(payloadPropertyName, BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(propertyInfo != null, $"{payloadPropertyName} exists in {nameof(AuthorizationRequestMessage)} type.");
            propertyInfo.SetValue(authorizationMessage, payload);
            return (AuthorizationRequestMessage)authorizationMessage;
        }
    }
}
