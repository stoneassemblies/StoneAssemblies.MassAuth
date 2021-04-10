// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusHostedService.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.Extensions.Hosting;

    /// <summary>
    ///     The bus hosted service.
    /// </summary>
    public sealed class BusHostedService : IHostedService
    {
        /// <summary>
        ///     The bus control.
        /// </summary>
        private readonly IBusControl busControl;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BusHostedService" /> class.
        /// </summary>
        /// <param name="busControl">
        ///     The bus control.
        /// </param>
        public BusHostedService(IBusControl busControl)
        {
            this.busControl = busControl;
        }

        /// <summary>
        ///     Starts the hosted service.
        /// </summary>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this.busControl.StartAsync(cancellationToken);
        }

        /// <summary>
        ///     Stops the hosted service.
        /// </summary>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this.busControl.StopAsync(cancellationToken);
        }
    }
}