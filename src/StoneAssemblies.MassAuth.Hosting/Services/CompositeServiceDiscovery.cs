#if NET5_0

namespace StoneAssemblies.MassAuth.Hosting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serilog;

    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;

    public class CompositeServiceDiscovery : IServiceDiscovery
    {
        private readonly List<IServiceDiscovery> serviceDiscoveries = new List<IServiceDiscovery>();

        public void Add(IServiceDiscovery serviceDiscovery)
        {
            this.serviceDiscoveries.Add(serviceDiscovery);
        }

        public Task<string> GetServiceEndPoint(string serviceName, string bindingName)
        {
            throw new NotImplementedException();
        }

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

        public Task<string> GetServiceEndPointAddressAsync(string serviceName, string bindingName, string protocol)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetServiceEndPointAsync(string serviceName)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetServiceEndPointAsync(string serviceName, string bindingName)
        {
            throw new NotImplementedException();
        }
    }
}
#endif