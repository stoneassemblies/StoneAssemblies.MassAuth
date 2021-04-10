namespace StoneAssemblies.MassAuth.Services.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    using MassTransit;

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
        ///     The create request client methods.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MethodInfo> CreateRequestClientMethods =
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
    }
}