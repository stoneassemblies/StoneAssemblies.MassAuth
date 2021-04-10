namespace StoneAssemblies.MassAuth.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    ///     The type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Get response method name.
        /// </summary>
        private const string GetResponseMethodName = "GetResponse";

        /// <summary>
        ///     Cache for the get response methods.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MethodInfo> GetResponseMethods =
            new ConcurrentDictionary<Type, MethodInfo>();

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