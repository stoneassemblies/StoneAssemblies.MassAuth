// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationRequestMessage.generic.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Messages.Interfaces
{
    /// <summary>
    ///     The AuthorizationRequestMessage interface.
    /// </summary>
    /// <typeparam name="TPayload">
    ///     The payload type.
    /// </typeparam>
    public interface IAuthorizationRequestMessage<out TPayload> : IAuthorizationRequestMessage
        where TPayload : MessageBase
    {
        /// <summary>
        ///     Gets the payload.
        /// </summary>
        TPayload Payload { get; }
    }
}