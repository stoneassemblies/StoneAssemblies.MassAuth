// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationRequestMessage.generic.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Messages
{
    using StoneAssemblies.MassAuth.Messages.Interfaces;

    /// <summary>
    ///     The authorization request message.
    /// </summary>
    /// <typeparam name="TPayload">
    ///     The payload type.
    /// </typeparam>
    public class AuthorizationRequestMessage<TPayload> : AuthorizationRequestMessage,
                                                         IAuthorizationRequestMessage<TPayload>
        where TPayload : MessageBase
    {
        /// <summary>
        ///     Gets or sets the payload.
        /// </summary>
        public TPayload Payload { get; set; }
    }
}