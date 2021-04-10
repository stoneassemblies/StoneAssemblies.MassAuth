// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExtensionManager.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// The ExtensionManager interface.
    /// </summary>
    public interface IExtensionManager
    {
        /// <summary>
        /// Gets the extension assemblies.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Assembly}"/>.
        /// </returns>
        IEnumerable<Assembly> GetExtensionAssemblies();

        /// <summary>
        /// Loads the extensions from package ids.
        /// </summary>
        /// <param name="packageIds">
        /// The package ids.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task LoadExtensionsAsync(List<string> packageIds);

        /// <summary>
        /// Loads extensions.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task LoadExtensionsAsync();
    }
}