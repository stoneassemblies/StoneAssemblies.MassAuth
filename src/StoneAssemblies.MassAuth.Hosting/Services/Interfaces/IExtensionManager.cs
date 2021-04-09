namespace StoneAssemblies.MassAuth.Hosting.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    public interface IExtensionManager
    {
        Task LoadExtensionsAsync(List<string> packageIds);

        Task LoadExtensionsAsync();

        IEnumerable<Assembly> GetExtensionAssemblies();
    }
}