// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KubernetesServiceDiscovery.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;

    /// <summary>
    ///     The kubernetes service discovery.
    /// </summary>
    public class KubernetesServiceDiscovery : IServiceDiscovery
    {
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
            var endPoint = await this.GetServiceEndPointAsync(serviceName);

            return $"{protocol}://{endPoint}";
        }

        /// <summary>
        ///     Gets service end point address async.
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
        public async Task<string> GetServiceEndPointAddressAsync(
            string serviceName,
            string bindingName,
            string protocol)
        {
            var endPoint = await this.GetServiceEndPointAsync(serviceName, bindingName);

            return $"{protocol}://{endPoint}";
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
        public Task<string> GetServiceEndPointAsync(string serviceName)
        {
            var serviceHost = Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_SERVICE_HOST");
            var servicePort = Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_SERVICE_PORT");

            return Task.FromResult($"{serviceHost}:{servicePort}");
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
        public Task<string> GetServiceEndPointAsync(string serviceName, string bindingName)
        {
            var serviceHost = Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_SERVICE_HOST")
                              ?? Environment.GetEnvironmentVariable(
                                  $"{serviceName.ToUpper()}_{bindingName.ToUpper()}_SERVICE_HOST");
            var servicePort =
                Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_SERVICE_PORT_{bindingName.ToUpper()}")
                ?? Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_{bindingName.ToUpper()}_SERVICE_PORT");

            return Task.FromResult($"{serviceHost}:{servicePort}");
        }
    }
}