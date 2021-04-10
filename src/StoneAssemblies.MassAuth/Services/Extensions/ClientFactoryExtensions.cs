namespace StoneAssemblies.MassAuth.Services.Extensions
{
    using System;

    using MassTransit;

    /// <summary>
    ///     The client factory extensions.
    /// </summary>
    public static class ClientFactoryExtensions
    {
        /// <summary>
        ///     Creates a request client.
        /// </summary>
        /// <param name="clientFactory">
        ///     The client factory.
        /// </param>
        /// <param name="messageType">
        ///     The message type.
        /// </param>
        /// <returns>
        ///     The <see cref="RequestClient" />.
        /// </returns>
        public static RequestClient CreateRequestClient(this IClientFactory clientFactory, Type messageType)
        {
            var methodInfo = clientFactory.GetType().GetCreateRequestClientMethodFromMessageType(messageType);
            var requestClient = methodInfo?.Invoke(clientFactory, new object[] { default(RequestTimeout) });
            return new RequestClient(requestClient);
        }
    }
}