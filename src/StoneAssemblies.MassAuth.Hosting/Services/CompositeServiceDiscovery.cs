// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeServiceDiscovery.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serilog;

    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;

    /// <summary>
    ///     The composite service discovery.
    /// </summary>
    public class CompositeServiceDiscovery : IServiceDiscovery
    {
        /// <summary>
        ///     The service discoveries.
        /// </summary>
        private readonly List<IServiceDiscovery> serviceDiscoveries = new List<IServiceDiscovery>();

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="serviceDiscovery">
        ///     The service discovery.
        /// </param>
        public void Add(IServiceDiscovery serviceDiscovery)
        {
            this.serviceDiscoveries.Add(serviceDiscovery);
        }

        /// <summary>
        ///     Gets the service end point.
        /// </summary>
        /// <param name="serviceName">
        ///     The service name.
        /// </param>
        /// <param name="bindingName">
        ///     The binding name.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        ///     This method is not implemented.
        /// </exception>
        public Task<string> GetServiceEndPoint(string serviceName, string bindingName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the service end point address async.
        /// </summary>
        /// <param name="serviceName">
        ///     The service name.
        /// </param>
        /// <param name="protocol">
        ///     The protocol.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<string> GetServiceEndPointAddressAsync(string serviceName, string protocol)
        {
            foreach (var serviceDiscovery in this.serviceDiscoveries)
            {
                try
                {
                    var serviceEndPoint = await serviceDiscovery.GetServiceEndPointAsync(serviceName, protocol);
                    if (!string.IsNullOrWhiteSpace(serviceEndPoint))
                    {
                        return serviceEndPoint;
                    }
                }
                catch (Exception e)
                {
                    Log.Warning(e, "Error discovering service '{ServiceName}'", serviceName);
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the service end point address async.
        /// </summary>
        /// <param name="serviceName">
        ///     The service name.
        /// </param>
        /// <param name="bindingName">
        ///     The binding name.
        /// </param>
        /// <param name="protocol">
        ///     The protocol.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        ///     This method is not implemented.
        /// </exception>
        public Task<string> GetServiceEndPointAddressAsync(string serviceName, string bindingName, string protocol)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the service end point.
        /// </summary>
        /// <param name="serviceName">
        ///     The service name.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        ///     This method is not implemented.
        /// </exception>
        public Task<string> GetServiceEndPointAsync(string serviceName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the service end point.
        /// </summary>
        /// <param name="serviceName">
        ///     The service name.
        /// </param>
        /// <param name="bindingName">
        ///     The binding name.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        ///     This method is not implemented.
        /// </exception>
        public Task<string> GetServiceEndPointAsync(string serviceName, string bindingName)
        {
            throw new NotImplementedException();
        }
    }
}