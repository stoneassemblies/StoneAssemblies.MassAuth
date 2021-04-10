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

    public class MarathonServiceDiscovery : IServiceDiscovery
    {
        private const string ServiceEndPoint = "/service/marathon/v2";

        private readonly string marathonServiceEndPoint;

        public MarathonServiceDiscovery()
            : this("http://master.mesos")
        {
        }

        public MarathonServiceDiscovery(string marathonServiceBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(marathonServiceBaseUrl))
            {
                throw new ArgumentException(nameof(marathonServiceBaseUrl));
            }

            this.marathonServiceEndPoint =
                new Uri(new Uri(marathonServiceBaseUrl.TrimEnd(' ', '/')), ServiceEndPoint).AbsoluteUri;
        }

        public Task<string> GetServiceEndPoint(string serviceName, string bindingName)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetServiceEndPointAddressAsync(string serviceName, string protocol)
        {
            var endPoint = await this.GetServiceEndPointAsync(serviceName);

            return $"{protocol}://{endPoint}";
        }

        public async Task<string> GetServiceEndPointAddressAsync(
            string serviceName,
            string bindingName,
            string protocol)
        {
            var endPoint = await this.GetServiceEndPointAsync(serviceName, bindingName);

            return $"{protocol}://{endPoint}";
        }

        public async Task<string> GetServiceEndPointAsync(string serviceName)
        {
            return await this.GetServiceEndPointAsync(serviceName, string.Empty);
        }

        public async Task<string> GetServiceEndPointAsync(string serviceName, string bindingName)
        {
            return await this.GetServiceEndPointFrom(this.marathonServiceEndPoint, serviceName, bindingName);
        }

        private Task<int> GetBindingNameIndexAsync(string endPoint, string serviceName, string bindingName)
        {
            // TODO: Implement this correctly?
            // var httpClient = new HttpClient();
            // var stringAsync = await httpClient.GetStringAsync($"{endPoint}/apps/{instanceId}/tasks");
            return Task.FromResult(0);
        }

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