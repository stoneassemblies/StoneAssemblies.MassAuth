namespace StoneAssemblies.MassAuth.Hosting.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IServiceDiscovery
    {
        Task<string> GetServiceEndPointAddressAsync(string serviceName, string protocol);

        Task<string> GetServiceEndPointAddressAsync(string serviceName, string bindingName, string protocol);

        Task<string> GetServiceEndPointAsync(string serviceName);

        Task<string> GetServiceEndPointAsync(string serviceName, string bindingName);
    }
}