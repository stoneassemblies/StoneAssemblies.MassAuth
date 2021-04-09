namespace StoneAssemblies.MassAuth.Hosting.Extensions
{
    using System.Linq;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.MassAuth.Hosting.Services;

    public static class IServiceCollectionExtensions
    {
        public static void AddExtensionManager(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            bool load = false)
        {
            var extensionManager = ExtensionManager.From(configuration);
            serviceCollection.AddSingleton(extensionManager);
            if (load)
            {
                extensionManager.LoadExtensionsAsync().GetAwaiter().GetResult();
            }
        }

        public static void AddServiceDiscovery(this IServiceCollection serviceCollection)
        {
            var serviceDiscovery = ServiceDiscoveryFactory.GetServiceDiscovery();
            serviceCollection.AddSingleton(serviceDiscovery);
        }

        public static TServiceType GetRegisteredInstance<TServiceType>(this IServiceCollection services)
        {
            var type = typeof(TServiceType);
            var serviceDescriptor = services.FirstOrDefault(
                descriptor => descriptor.ServiceType == type && descriptor.ImplementationInstance != null);
            var implementationInstance = serviceDescriptor?.ImplementationInstance;

            return (TServiceType)implementationInstance;
        }
    }
}