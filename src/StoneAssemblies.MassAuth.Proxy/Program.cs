// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Proxy
{
    using System;
    using System.IO;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    using Serilog;

    /// <summary>
    ///     The program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     Creates host builder.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="IHostBuilder" />.
        /// </returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }

        /// <summary>
        ///     The main entry point.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            // TODO: Move this to another place. Is duplicated.
            CreateHostBuilder(args).ConfigureAppConfiguration(
                builder =>
                    {
                        var environmentPath =
#if !DEBUG
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json";
#else
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json";
#endif
                        builder.SetBasePath(Directory.GetCurrentDirectory());
                        builder.AddJsonFile("appsettings.json", true, true);

                        Log.Information("Adding configuration file '{EnvironmentPath}'", environmentPath);
                        builder.AddJsonFile(environmentPath, true, true);

                        Log.Information("Adding configuration from environment variables");
                        builder.AddEnvironmentVariables();
                    }).Build().Run();
        }
    }
}