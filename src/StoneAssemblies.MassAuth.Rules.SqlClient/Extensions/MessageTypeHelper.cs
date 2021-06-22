// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageTypeHelper.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Extensions
{
    using System;
    using System.Linq;

    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    ///     The message type helper.
    /// </summary>
    public static class MessageTypeHelper
    {
        /// <summary>
        ///     Gets message type.
        /// </summary>
        /// <param name="messageTypeName">
        ///     The message type name.
        /// </param>
        /// <returns>
        ///     The <see cref="Type" />.
        /// </returns>
        public static Type GetMessageType(string messageTypeName)
        {
            var messageType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                assembly =>
                    {
                        try
                        {
                            return assembly.GetTypes();
                        }
                        catch (Exception)
                        {
                            return Array.Empty<Type>();
                        }
                    }).FirstOrDefault(type => typeof(MessageBase).IsAssignableFrom(type) && type.Name == messageTypeName);
            return messageType;
        }
    }
}