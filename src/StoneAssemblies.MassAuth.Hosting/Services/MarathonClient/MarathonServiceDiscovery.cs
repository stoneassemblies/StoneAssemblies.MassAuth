// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MarathonServiceDiscovery.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services.MarathonClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    using Serilog;

    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;
    using StoneAssemblies.MassAuth.Hosting.Services.MarathonClient.Models;

    /// <summary>
    ///     The marathon service discovery.
    /// </summary>
    public class MarathonServiceDiscovery : IServiceDiscovery
    {
        /// <summary>
        ///     The service end point.
        /// </summary>
        private const string ServiceEndPoint = "/service/marathon/v2";

        /// <summary>
        ///     The marathon service end point.
        /// </summary>
        private readonly string marathonServiceEndPoint;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarathonServiceDiscovery" /> class.
        /// </summary>
        public MarathonServiceDiscovery()
            : this("http://master.mesos")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarathonServiceDiscovery" /> class.
        /// </summary>
        /// <param name="marathonServiceBaseUrl">
        ///     The marathon service base url.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Invalid arguments.
        /// </exception>
        public MarathonServiceDiscovery(string marathonServiceBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(marathonServiceBaseUrl))
            {
                throw new ArgumentException(nameof(marathonServiceBaseUrl));
            }

            this.marathonServiceEndPoint =
                new Uri(new Uri(marathonServiceBaseUrl.TrimEnd(' ', '/')), ServiceEndPoint).AbsoluteUri;
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
        ///     Gets the service end point async.
        /// </summary>
        /// <param name="serviceName">
        ///     The service name.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<string> GetServiceEndPointAsync(string serviceName)
        {
            return await this.GetServiceEndPointAsync(serviceName, string.Empty);
        }

        /// <summary>
        ///     Gets the service end point async.
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
        public async Task<string> GetServiceEndPointAsync(string serviceName, string bindingName)
        {
            return await this.GetServiceEndPointFrom(this.marathonServiceEndPoint, serviceName, bindingName);
        }

        /// <summary>
        ///     Gets the binding name index async.
        /// </summary>
        /// <param name="endPoint">
        ///     The end point.
        /// </param>
        /// <param name="serviceName">
        ///     The service name.
        /// </param>
        /// <param name="bindingName">
        ///     The binding name.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        private Task<int> GetBindingNameIndexAsync(string endPoint, string serviceName, string bindingName)
        {
            // TODO: Implement this correctly?
            // var httpClient = new HttpClient();
            // var stringAsync = await httpClient.GetStringAsync($"{endPoint}/apps/{instanceId}/tasks");
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Gets the service end point from.
        /// </summary>
        /// <param name="marathonEndPoint">
        ///     The marathon end point.
        /// </param>
        /// <param name="serviceName">
        ///     The service name.
        /// </param>
        /// <param name="bindingName">
        ///     The binding name.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Incorrect arguments.
        /// </exception>
        private async Task<string> GetServiceEndPointFrom(
            string marathonEndPoint,
            string serviceName,
            string bindingName = "")
        {
            if (string.IsNullOrWhiteSpace(marathonEndPoint))
            {
                throw new ArgumentException(nameof(marathonEndPoint));
            }

            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentException(nameof(serviceName));
            }

            // TODO: Remove this if is possible.
            if (!serviceName.StartsWith('/'))
            {
                serviceName = "/" + serviceName.Trim();
            }

            Log.Debug("Getting service '{serviceName}' end point...", serviceName);

            var serviceTasks = await this.GetTasksAsync(marathonEndPoint, string.Empty, serviceName);
            var serviceTask = serviceTasks.FirstOrDefault();

            // TODO: Improve this later
            var idx = await this.GetBindingNameIndexAsync(marathonEndPoint, serviceName, bindingName);

            if (serviceTask != null && idx < serviceTask.Ports.Count)
            {
                return $"{serviceTask.Host}:{serviceTask.Ports[idx]}";
            }

            return string.Empty;
        }

        /// <summary>
        ///     Gets tasks.
        /// </summary>
        /// <param name="endPoint">
        ///     The end point.
        /// </param>
        /// <param name="groupId">
        ///     The group id.
        /// </param>
        /// <param name="instanceId">
        ///     The instance id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        private async Task<List<ServiceTask>> GetTasksAsync(string endPoint, string groupId, string instanceId)
        {
            var httpClient = new HttpClient();
            ServiceTasks serviceTasks;
            if (!string.IsNullOrWhiteSpace(groupId))
            {
                serviceTasks =
                    await httpClient.GetFromJsonAsync<ServiceTasks>($"{endPoint}/apps/{groupId}/{instanceId}/tasks");
            }
            else
            {
                serviceTasks = await httpClient.GetFromJsonAsync<ServiceTasks>($"{endPoint}/apps/{instanceId}/tasks");
            }

            return serviceTasks.Tasks;
        }
    }
}