namespace StoneAssemblies.MassAuth.Services.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using MassTransit;

    public static class IClientFactoryExtensions
    {
        private static readonly Dictionary<Type, MethodInfo> CreateRequestClientMethods =
            new Dictionary<Type, MethodInfo>();

        private static readonly object SyncObj = new object();

        public static RequestClient CreateRequestClient(this IClientFactory clientFactory, Type messageType)
        {
            MethodInfo methodInfo;
            lock (SyncObj)
            {
                if (!CreateRequestClientMethods.TryGetValue(messageType, out methodInfo))
                {
                    methodInfo = clientFactory.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .FirstOrDefault(
                            info => info.Name == "CreateRequestClient" && info.GetParameters().Length == 1
                                                                       && info.GetParameters()[0].ParameterType
                                                                       == typeof(RequestTimeout));

                    methodInfo = methodInfo?.MakeGenericMethod(messageType);
                    CreateRequestClientMethods[messageType] = methodInfo;
                }
            }

            var requestClient = methodInfo?.Invoke(clientFactory, new object[] { default(RequestTimeout) });
            return new RequestClient(requestClient);
        }
    }
}