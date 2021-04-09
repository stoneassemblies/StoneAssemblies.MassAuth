namespace StoneAssemblies.MassAuth.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;

    using StoneAssemblies.MassAuth.Messages;

    public class RequestClient
    {
        private static readonly Dictionary<Type, MethodInfo> GetResponseMethods = new Dictionary<Type, MethodInfo>();

        private static readonly object SyncObj = new object();

        private readonly object _requestClient;

        public RequestClient(object requestClient)
        {
            this._requestClient = requestClient;
        }

        public async Task<Response<T>> GetResponse<T>(object message)
            where T : class
        {
            MethodInfo methodInfo;
            lock (SyncObj)
            {
                var requestClientType = this._requestClient.GetType();
                if (!GetResponseMethods.TryGetValue(requestClientType, out methodInfo))
                {
                    methodInfo = requestClientType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(
                            info => info.Name == "GetResponse"
                                    && info.GetParameters()[0].ParameterType == message.GetType());
                    methodInfo = methodInfo?.MakeGenericMethod(typeof(AuthorizationResponseMessage));
                    GetResponseMethods[requestClientType] = methodInfo;
                }
            }

            var invoke = (Task<Response<T>>)methodInfo?.Invoke(
                this._requestClient,
                new[] { message, default(CancellationToken), default(RequestTimeout) });
            return invoke != null ? await invoke : null;
        }
    }
}