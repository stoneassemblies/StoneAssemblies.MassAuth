﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Proxy
{
    using System.Linq;
    using System.Reflection;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Logging;

    using StoneAssemblies.Extensibility;
    using StoneAssemblies.Hosting.Extensions;
    using StoneAssemblies.Hosting.Services;
    using StoneAssemblies.MassAuth.Extensions;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Proxy.Services;
    using StoneAssemblies.MassAuth.Services.Extensions;

    /// <summary>
    ///     The startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">
        ///     The app.
        /// </param>
        /// <param name="env">
        ///     The env.
        /// </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            IdentityModelEventSource.ShowPII = true;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/health");

            app.UseRouting();

            // TODO: Try to recover the 
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                });
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The services.
        /// </param>
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHealthChecks();

            var serviceDiscovery = ServiceDiscoveryFactory.GetServiceDiscovery();
            serviceCollection.AddSingleton(serviceDiscovery);

            serviceCollection.AddExtensionPackages(this.Configuration);
            var extensionManager = serviceCollection.GetRegisteredInstance<IExtensionManager>();

            serviceCollection.AddMassAuth(
                options =>
                {
                    // options.ReturnForbiddanceReason = true;
                });

            // TODO: Use service discovery to resolve address from service name.
            // var serviceDiscovery = serviceCollection.GetRegisteredInstance<IServiceDiscovery>();
            var username = this.Configuration.GetSection("RabbitMQ")?["Username"] ?? "queuedemo";
            var password = this.Configuration.GetSection("RabbitMQ")?["Password"] ?? "queuedemo";
            var messageQueueAddress = this.Configuration.GetSection("RabbitMQ")?["Address"] ?? "rabbitmq://localhost";

            serviceCollection.AddMassTransit(
                sc =>
                    {
                        sc.AddBus(
                            context => Bus.Factory.CreateUsingRabbitMq(
                                cfg =>
                                    {
                                        cfg.Host(
                                            messageQueueAddress,
                                            configurator =>
                                                {
                                                    configurator.Username(username);
                                                    configurator.Password(password);
                                                });

                                        cfg.ConfigureJsonSerializerOptions(
                                            options =>
                                            {
                                                options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                                                return options;
                                            });
                                    }));

                        var serviceCollectionBusConfiguratorExtensionsType = typeof(ServiceCollectionBusConfiguratorExtensions);
                        var addDefaultAuthorizationRequestClientMethodInfo = serviceCollectionBusConfiguratorExtensionsType.GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(info => info.Name == nameof(ServiceCollectionBusConfiguratorExtensions.AddDefaultAuthorizationRequestClient));

                        var extensionAssemblies = extensionManager.GetExtensionPackageAssemblies();
                        foreach (var extensionAssembly in extensionAssemblies)
                        {
                            var messageTypes = extensionAssembly.GetTypes().Where(type => typeof(MessageBase).IsAssignableFrom(type))
                                .ToList();
                            foreach (var type in messageTypes)
                            {
                                var makeGenericMethod = addDefaultAuthorizationRequestClientMethodInfo.MakeGenericMethod(type);
                                makeGenericMethod.Invoke(
                                    serviceCollectionBusConfiguratorExtensionsType,
                                    new object[] { sc, default(RequestTimeout) });
                            }
                        }
                    });

            serviceCollection.AddMvc(options => options.Conventions.Add(new GenericAuthorizeControllerRouteConvention()))
                .ConfigureApplicationPartManager(
                    manager => manager.FeatureProviders.Add(
                        new MessageTypeGenericAuthorizeControllerFeatureProvider(extensionManager)));

        }
    }
}