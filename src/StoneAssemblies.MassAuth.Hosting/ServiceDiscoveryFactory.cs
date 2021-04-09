namespace StoneAssemblies.MassAuth.Hosting
{
    using StoneAssemblies.MassAuth.Hosting.Services;
    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;
    using StoneAssemblies.MassAuth.Hosting.Services.MarathonClient;

    public static class ServiceDiscoveryFactory
    {
        public static IServiceDiscovery GetServiceDiscovery()
        {
            if (HostingEnvironment.IsMarathonHosted())
            {
                return new MarathonServiceDiscovery();
            }

            return new KubernetesServiceDiscovery();
        }
    }
}