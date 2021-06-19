// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    using MassTransit;

    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    ///     The type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     The create request client method name.
        /// </summary>
        private const string CreateRequestClientMethodName = "CreateRequestClient";

        /// <summary>
        ///     Get response method name.
        /// </summary>
        private const string GetResponseMethodName = "GetResponse";

        /// <summary>
        ///     The create request client methods.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MethodInfo> CreateRequestClientMethods =
            new ConcurrentDictionary<Type, MethodInfo>();

        /// <summary>
        ///     Cache for the get response methods.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MethodInfo> GetResponseMethods =
            new ConcurrentDictionary<Type, MethodInfo>();

        /// <summary>
        ///     Gets method create request client from message type.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="messageType">
        ///     The message type.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo" />.
        /// </returns>
        public static MethodInfo GetCreateRequestClientMethodFromMessageType(this Type type, Type messageType)
        {
            var methodInfo = CreateRequestClientMethods.GetOrAdd(messageType, type.GetCreateRequestClientMethod);
            return methodInfo;
        }

        /// <summary>
        ///     Gets the response method info from message type.
        /// </summary>
        /// <param name="requestClientType">
        ///     The request client type.
        /// </param>
        /// <param name="messageType">
        ///     The message type.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo" />.
        /// </returns>
        public static MethodInfo GetResponseMethodInfoFromMessageType(this Type requestClientType, Type messageType)
        {
            var methodInfo = GetResponseMethods.GetOrAdd(requestClientType, t => t.GetResponseMethodInfo(messageType));
            return methodInfo;
        }

        /// <summary>
        ///     Gets the create request client method.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="messageType">
        ///     The message type.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo" />.
        /// </returns>
        private static MethodInfo GetCreateRequestClientMethod(this Type type, Type messageType)
        {
            var method = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(
                info => info.Name == CreateRequestClientMethodName && info.GetParameters().Length == 1
                                                                   && info.GetParameters()[0].ParameterType
                                                                   == typeof(RequestTimeout));
            method = method?.MakeGenericMethod(messageType);
            return method;
        }

        /// <summary>
        ///     The make generic method of get response.
        /// </summary>
        /// <param name="requestClientType">
        ///     The request client type.
        /// </param>
        /// <param name="messageType">
        ///     The message type.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo" />.
        /// </returns>
        private static MethodInfo GetResponseMethodInfo(this Type requestClientType, Type messageType)
        {
            var methodInfos = requestClientType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var methodInfo = methodInfos.FirstOrDefault(
                info => info.Name == GetResponseMethodName && info.GetParameters()[0].ParameterType == messageType);
            methodInfo = methodInfo?.MakeGenericMethod(typeof(AuthorizationResponseMessage));
            return methodInfo;
        }
    }
}
