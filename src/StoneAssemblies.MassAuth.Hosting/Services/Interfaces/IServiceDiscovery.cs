// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceDiscovery.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    ///     The ServiceDiscovery interface.
    /// </summary>
    public interface IServiceDiscovery
    {
        /// <summary>
        ///     Gets service end point address async.
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
        Task<string> GetServiceEndPointAddressAsync(string serviceName, string protocol);

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
        Task<string> GetServiceEndPointAddressAsync(string serviceName, string bindingName, string protocol);

        /// <summary>
        ///     Gets service end point async.
        /// </summary>
        /// <param name="serviceName">
        ///     The service name.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        Task<string> GetServiceEndPointAsync(string serviceName);

        /// <summary>
        ///     Gets service end point async.
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
        Task<string> GetServiceEndPointAsync(string serviceName, string bindingName);
    }
}