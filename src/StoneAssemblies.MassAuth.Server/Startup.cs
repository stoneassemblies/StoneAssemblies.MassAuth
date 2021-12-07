// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable
namespace StoneAssemblies.MassAuth.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;

    using GreenPipes;

    using MassTransit;
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus;
    using MassTransit.MultiBus;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Newtonsoft.Json;

    using Serilog;

    using StoneAssemblies.Contrib.MassTransit.Extensions;
    using StoneAssemblies.Extensibility.Extensions;
    using StoneAssemblies.Hosting.Extensions;
    using StoneAssemblies.MassAuth.Extensions;
    using StoneAssemblies.MassAuth.Hosting.Extensions;
    using StoneAssemblies.MassAuth.Hosting.Services;
    using StoneAssemblies.MassAuth.Services;

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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/health");

            app.UseRouting();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
                    });
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <remarks>
        ///     For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </remarks>
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHealthChecks();
            serviceCollection.AddServiceDiscovery();
            serviceCollection.AddExtensions(this.Configuration);
            serviceCollection.AddRules();

            // TODO: Use service discovery to resolve address from service name.
            // var serviceDiscovery = serviceCollection.GetRegisteredInstance<IServiceDiscovery>();
            var username = this.Configuration.GetSection("RabbitMQ")?["Username"] ?? "queuedemo";
            var password = this.Configuration.GetSection("RabbitMQ")?["Password"] ?? "queuedemo";
            var virtualHosts = new List<string>();
            this.Configuration.GetSection("RabbitMQ")?.GetSection("VirtualHosts").Bind(virtualHosts);
            var messageQueueAddress = this.Configuration.GetSection("RabbitMQ")?["Address"] ?? "rabbitmq://localhost";
            foreach (var virtualHost in virtualHosts)
            {
                serviceCollection.AddMassTransit(
                    $"{virtualHost}Bus",
                    serviceCollectionConfigurator =>
                        {
                            AddBus(serviceCollectionConfigurator, messageQueueAddress, virtualHost, username, password);
                        });
            }

            serviceCollection.AddMassTransitHostedService();
        }

        private static void AddBus(IServiceCollectionBusConfigurator serviceCollectionConfigurator, string messageQueueAddress, string? virtualHost, string username, string password)
        {
            serviceCollectionConfigurator.AddAuthorizationRequestConsumers();

            Log.Information("Connecting to message queue server with address '{ServiceAddress}'", messageQueueAddress);

            serviceCollectionConfigurator.AddBus(
                context => Bus.Factory.CreateUsingRabbitMq(
                    cfg =>
                        {
                            if (!string.IsNullOrEmpty(virtualHost))
                            {
                                cfg.Host(
                                    new Uri(new Uri(messageQueueAddress), new Uri(virtualHost, UriKind.Relative)),
                                    configurator =>
                                        {
                                            configurator.Username(username);
                                            configurator.Password(password);
                                        });
                            }
                            else
                            {
                                cfg.Host(
                                    messageQueueAddress,
                                    configurator =>
                                        {
                                            configurator.Username(username);
                                            configurator.Password(password);
                                        });
                            }

                            cfg.ConfigureJsonSerializer(
                                s =>
                                    {
                                        s.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                                        return s;
                                    });

                            cfg.ConfigureJsonDeserializer(
                                s =>
                                    {
                                        s.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                                        s.Converters.Add(new ClaimConverter());
                                        return s;
                                    });

                            serviceCollectionConfigurator.ConfigureAuthorizationRequestConsumers(
                                (messagesType, consumerType) =>
                                    {
                                        cfg.DefaultReceiveEndpoint(
                                            messagesType,
                                            e =>
                                                {
                                                    e.PrefetchCount = 16;
                                                    e.UseMessageRetry(x => x.Interval(2, 100));
                                                    e.ConfigureConsumer(context, consumerType);
                                                });
                                    });
                        }));
        }
    }
}