namespace StoneAssemblies.MassAuth.Hosting.Services
{
    using System;
    using System.Threading.Tasks;

    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;

    public class KubernetesServiceDiscovery : IServiceDiscovery
    {
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
            var serviceHost = Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_SERVICE_HOST");
            var servicePort = Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_SERVICE_PORT");

            return $"{serviceHost}:{servicePort}";
        }

        public async Task<string> GetServiceEndPointAsync(string serviceName, string bindingName)
        {
            var serviceHost = Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_SERVICE_HOST")
                              ?? Environment.GetEnvironmentVariable(
                                  $"{serviceName.ToUpper()}_{bindingName.ToUpper()}_SERVICE_HOST");
            var servicePort =
                Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_SERVICE_PORT_{bindingName.ToUpper()}")
                ?? Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_{bindingName.ToUpper()}_SERVICE_PORT");

            return $"{serviceHost}:{servicePort}";
        }
    }
}