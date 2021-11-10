// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestClientExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services.Extensions
{
    using System;
    using System.Threading.Tasks;

    using Serilog;

    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    /// The request client extensions.
    /// </summary>
    public static class RequestClientExtensions
    {
        /// <summary>
        /// The get authorization response message async.
        /// </summary>
        /// <param name="clientRequest">
        /// The client request.
        /// </param>
        /// <param name="authorizationMessage">
        /// The authorization message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<AuthorizationResponseMessage> GetAuthorizationResponseMessageAsync(
            this RequestClient clientRequest, AuthorizationRequestMessage authorizationMessage)
        {
            AuthorizationResponseMessage responseMessage;
            try
            {
                var response = await clientRequest.GetResponse<AuthorizationResponseMessage>(authorizationMessage);
                responseMessage = response.Message;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error requesting authorization for message type '{MessageType}'", authorizationMessage.GetType());

                responseMessage = new AuthorizationResponseMessage
                                      {
                                          IsAuthorized = false,
                                          ForbiddanceReason = "Error requesting authorization",
                                      };
            }

            return responseMessage;
        }
    }
}