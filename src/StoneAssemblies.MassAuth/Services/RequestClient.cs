// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestClient.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;

    using StoneAssemblies.MassAuth.Extensions;

    /// <summary>
    ///     The request client.
    /// </summary>
    public class RequestClient
    {
        /// <summary>
        ///     The request client.
        /// </summary>
        private readonly object requestClient;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RequestClient" /> class.
        /// </summary>
        /// <param name="requestClient">
        ///     The request client.
        /// </param>
        public RequestClient(object requestClient)
        {
            this.requestClient = requestClient;
        }

        /// <summary>
        ///     Gets the response.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <typeparam name="TMessage">
        ///     The message type.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<Response<TMessage>> GetResponse<TMessage>(object message)
            where TMessage : class
        {
            var requestClientType = this.requestClient.GetType();
            var methodInfo = requestClientType.GetResponseMethodInfoFromMessageType(message.GetType());
            var invoke = (Task<Response<TMessage>>)methodInfo?.Invoke(
                this.requestClient,
                new[] { message, default(CancellationToken), default(RequestTimeout) });
            return invoke != null ? await invoke : null;
        }
    }
}
