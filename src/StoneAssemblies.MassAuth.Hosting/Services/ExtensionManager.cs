#nullable enable
namespace StoneAssemblies.MassAuth.Hosting.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

    using NuGet.Common;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;

    using Serilog;

    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;

    /// <summary>
    ///     The extension manager.
    /// </summary>
    public class ExtensionManager : IExtensionManager
    {
        /// <summary>
        ///     The cache directory folder name.
        /// </summary>
        private const string CacheDirectoryFolderName = "cache";

        /// <summary>
        ///     The dependencies directory folder name.
        /// </summary>
        private const string DependenciesDirectoryFolderName = "lib";

        /// <summary>
        ///     The plugins directory folder name.
        /// </summary>
        private const string PluginsDirectoryFolderName = "plugins";

        /// <summary>
        ///     The package library directory names.
        /// </summary>
        private static readonly string[] PackageLibraryDirectoryNames =
            {
                Path.Combine("runtimes", "$(Platform)"), "lib"
            };

        /// <summary>
        ///     The target framework dependencies.
        /// </summary>
        private static readonly string[] TargetFrameworkDependencies =
            {
                ".NETCoreApp,Version=v5.0", ".NETCoreApp,Version=v3.1", ".NetStandard,Version=v2.1", ".NetStandard,Version=v2.0"
            };

        /// <summary>
        ///     The target frameworks.
        /// </summary>
        private static readonly string[] TargetFrameworks =
            {
                "net5.0", "netcoreapp3.1", "netstandard2.1", "netstandard2.0", "net46"
            };

        /// <summary>
        ///     The configuration.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        ///     The extensions.
        /// </summary>
        private readonly List<Assembly> extensions = new List<Assembly>();

        /// <summary>
        ///     The repository.
        /// </summary>
        private readonly SourceRepository? repository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtensionManager" /> class.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        private ExtensionManager(IConfiguration configuration)
        {
            AssemblyLoadContext.Default.ResolvingUnmanagedDll += this.OnAssemblyLoadContextResolvingUnmanagedDll;
            AppDomain.CurrentDomain.AssemblyResolve += this.OnCurrentAppDomainAssemblyResolve;

            this.configuration = configuration;
            var repositoryUrl = this.configuration.GetSection("Extensions")["RepositoryUrl"];
            if (!string.IsNullOrWhiteSpace(repositoryUrl))
            {
                if (!Uri.TryCreate(repositoryUrl, UriKind.Absolute, out _) && Directory.Exists(repositoryUrl))
                {
                    repositoryUrl = Path.GetFullPath(repositoryUrl);
                }

                this.repository = Repository.Factory.GetCoreV3(repositoryUrl);
            }
        }

        /// <summary>
        ///     Creates an extension manager from a configuration.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The <see cref="IExtensionManager" />.
        /// </returns>
        public static IExtensionManager From(IConfiguration configuration)
        {
            return new ExtensionManager(configuration);
        }

        /// <summary>
        ///     Gets the extension assemblies.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable{Assembly}" />.
        /// </returns>
        public IEnumerable<Assembly> GetExtensionAssemblies()
        {
            foreach (var extension in this.extensions)
            {
                yield return extension;
            }
        }

        /// <summary>
        ///     Loads the extensions.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task LoadExtensionsAsync()
        {
            var packageIds = new List<string>();
            this.configuration.GetSection("Extensions").GetSection("Packages").Bind(packageIds);
            await this.LoadExtensionsAsync(packageIds);
        }

        /// <summary>
        ///     Loads the extensions from package ids.
        /// </summary>
        /// <param name="packageIds">
        ///     The package ids.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task LoadExtensionsAsync(List<string> packageIds)
        {
            if (this.repository != null)
            {
                var resource = await this.repository.GetResourceAsync<FindPackageByIdResource>();
                foreach (var id in packageIds)
                {
                    var packageId = id;
                    NuGetVersion? packageVersion = null;

                    var packageIdParts = id.Split(':');
                    if (packageIdParts.Length == 2)
                    {
                        packageId = packageIdParts[0];
                        NuGetVersion.TryParse(packageIdParts[1], out packageVersion);
                    }

                    if (packageVersion == null)
                    {
                        var versions = await resource.GetAllVersionsAsync(
                                           packageId,
                                           new NullSourceCacheContext(),
                                           NullLogger.Instance,
                                           CancellationToken.None);

                        packageVersion = versions.ToList().LastOrDefault();
                    }

                    if (packageVersion != null)
                    {
                        var pluginsDirectoryPath = Path.GetFullPath(PluginsDirectoryFolderName);
                        await this.DownloadPackageAsync(
                            new PackageDependency(packageId, new VersionRange(packageVersion)),
                            resource,
                            pluginsDirectoryPath);

                        var packageDirectoryName = $"{packageId}.{packageVersion.OriginalVersion}";
                        var pluginDirectoryPath = Path.Combine(pluginsDirectoryPath, packageDirectoryName);

                        // TODO: Update?
                        foreach (var targetFramework in TargetFrameworks)
                        {
                            var targetFrameworkDirectoryPath = Path.Combine(
                                pluginDirectoryPath,
                                "lib",
                                targetFramework);
                            if (Directory.Exists(targetFrameworkDirectoryPath))
                            {
                                var assemblyFiles = Directory.EnumerateFiles(targetFrameworkDirectoryPath, "*.dll")
                                    .ToList();

                                if (assemblyFiles.Count > 0)
                                {
                                    try
                                    {
                                        foreach (var assemblyFile in assemblyFiles)
                                        {
                                            this.extensions.Add(Assembly.LoadFrom(assemblyFile));
                                        }

                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error(ex, "Error loading plugin assembly");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the runtime id.
        /// </summary>
        /// <returns>
        ///     The runtime id.
        /// </returns>
        private static string GetRuntimeId()
        {
            //// TODO: Add runtime ids if required.
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" : "unix";
        }

        /// <summary>
        ///     Downloads the package async.
        /// </summary>
        /// <param name="package">
        ///     The package.
        /// </param>
        /// <param name="resource">
        ///     The resource.
        /// </param>
        /// <param name="destination">
        ///     The destination.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        private async Task DownloadPackageAsync(
            PackageDependency package,
            FindPackageByIdResource resource,
            string destination)
        {
            var packageId = package.Id;
            var packageDependencyVersions = await resource.GetAllVersionsAsync(
                                                packageId,
                                                new NullSourceCacheContext(),
                                                NullLogger.Instance,
                                                CancellationToken.None);

            var packageVersion = package.VersionRange.FindBestMatch(packageDependencyVersions);
            if (packageVersion != null)
            {
                if (!Directory.Exists(CacheDirectoryFolderName))
                {
                    Directory.CreateDirectory(CacheDirectoryFolderName);
                }

                var packageFileName = Path.Combine(
                    CacheDirectoryFolderName,
                    $"{packageId}.{packageVersion.OriginalVersion}.nupkg");
                var packageDirectoryName = Path.GetFileNameWithoutExtension(packageFileName);
                if (!File.Exists(packageFileName))
                {
                    using (var packageStream = new FileStream(packageFileName, FileMode.Create, FileAccess.Write))
                    {
                        Log.Information(
                            "Downloading dependency package {PackageId} {PackageVersion}",
                            packageId,
                            packageVersion.OriginalVersion);

                        await resource.CopyNupkgToStreamAsync(
                            packageId,
                            packageVersion,
                            packageStream,
                            new NullSourceCacheContext(),
                            NullLogger.Instance,
                            CancellationToken.None);
                    }
                }

                using (var archiveReader = new PackageArchiveReader(packageFileName))
                {
                    foreach (var dependencyGroup in archiveReader.GetPackageDependencies())
                    {
                        if (TargetFrameworkDependencies.Contains(
                            dependencyGroup.TargetFramework.DotNetFrameworkName,
                            StringComparer.InvariantCultureIgnoreCase))
                        {
                            foreach (var packageDependency in dependencyGroup.Packages)
                            {
                                try
                                {
                                    await this.DownloadPackageAsync(
                                        packageDependency,
                                        resource,
                                        DependenciesDirectoryFolderName);
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e, "Error downloading package {PackageId}", packageDependency.Id);
                                }
                            }

                            break;
                        }
                    }
                }

                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }

                var packagesDirectoryPath = Path.Combine(destination, packageDirectoryName);
                if (!Directory.Exists(packagesDirectoryPath))
                {
                    ZipFile.ExtractToDirectory(packageFileName, packagesDirectoryPath, true);
                }
            }
        }

        /// <summary>
        ///     Called on assembly load context resolving unmanaged library.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <param name="libraryFileName">
        ///     The library file name.
        /// </param>
        /// <returns>
        ///     The <see cref="IntPtr" />.
        /// </returns>
        private IntPtr OnAssemblyLoadContextResolvingUnmanagedDll(Assembly assembly, string libraryFileName)
        {
            var dependencyDirectoryFullPath = Path.GetFullPath(DependenciesDirectoryFolderName);
            var libraryPaths = Directory.EnumerateFiles(
                dependencyDirectoryFullPath,
                libraryFileName,
                SearchOption.AllDirectories);

            var unmanagedDll = IntPtr.Zero;
            foreach (var libraryPath in libraryPaths)
            {
                try
                {
                    Log.Information(
                        "Loading native dependency '{LibraryFileName}' from '{LibraryPath}'",
                        libraryFileName,
                        libraryPath);

                    unmanagedDll = NativeLibrary.Load(libraryPath);

                    Log.Information(
                        "Native dependency '{LibraryFileName}' was loaded successfully from '{LibraryPath} ",
                        libraryFileName,
                        libraryPath);

                    break;
                }
                catch (Exception ex)
                {
                    Log.Warning(
                        ex,
                        "Error loading '{LibraryFileName}' from '{LibraryPath}'",
                        libraryFileName,
                        libraryPath);
                }
            }

            return unmanagedDll;
        }

        /// <summary>
        ///     The on current app domain assembly resolve.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="Assembly?" />.
        /// </returns>
        private Assembly? OnCurrentAppDomainAssemblyResolve(object? sender, ResolveEventArgs args)
        {
            // TODO: Take in account culture for resource assemblies.
            var fileName = args.Name.Split(',')[0];
            var dependencyDirectoryFullPath = Path.GetFullPath(DependenciesDirectoryFolderName);
            var packageDirectories = Directory
                .EnumerateFiles(dependencyDirectoryFullPath, fileName + ".dll", SearchOption.AllDirectories).GroupBy(
                    f => f.Substring(0, f.IndexOf(Path.DirectorySeparatorChar, dependencyDirectoryFullPath.Length + 1)))
                .ToList();

            foreach (IGrouping<string, string> grouping in packageDirectories)
            {
                var packageDirectory = grouping.Key;
                foreach (var directoryName in PackageLibraryDirectoryNames)
                {
                    var packageLibraryDirectoryName = directoryName.Replace("$(Platform)", GetRuntimeId());
                    var packageLibraryDirectoryPath = Path.Combine(packageDirectory, packageLibraryDirectoryName);
                    if (Directory.Exists(packageLibraryDirectoryPath))
                    {
                        var assemblyFiles = Directory.EnumerateFiles(
                            packageLibraryDirectoryPath,
                            fileName + ".dll",
                            SearchOption.AllDirectories).ToList();

                        foreach (var targetFramework in TargetFrameworks)
                        {
                            foreach (var assemblyFile in assemblyFiles)
                            {
                                try
                                {
                                    Log.Information(
                                        "Loading assembly '{AssemblyName}' from {AssemblyFile}",
                                        args.Name,
                                        assemblyFile);

                                    if (assemblyFile.Contains(
                                        $"{Path.DirectorySeparatorChar}{targetFramework}{Path.DirectorySeparatorChar}"))
                                    {
                                        var assembly = Assembly.LoadFrom(assemblyFile);

                                        Log.Information(
                                            "Assembly '{AssemblyName}' was loaded successfully from '{AssemblyFile} ",
                                            args.Name,
                                            assemblyFile);

                                        return assembly;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Warning(
                                        ex,
                                        "Error loading assembly '{AssemblyName}' file {AssemblyFile}",
                                        args.Name,
                                        assemblyFile);
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}