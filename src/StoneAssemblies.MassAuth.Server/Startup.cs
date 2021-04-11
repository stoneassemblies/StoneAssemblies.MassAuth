// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Server
{
    using GreenPipes;

    using MassTransit;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Newtonsoft.Json;

    using Serilog;

    using StoneAssemblies.MassAuth.Hosting.Extensions;
    using StoneAssemblies.MassAuth.Messages.Extensions;
    using StoneAssemblies.MassAuth.Server.Extensions;
    using StoneAssemblies.MassAuth.Server.Services;
    using StoneAssemblies.MassAuth.Server.Services.Interfaces;
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

            serviceCollection.AddExtensionManager(this.Configuration, true);

            serviceCollection.AddSingleton(typeof(IRulesContainer<>), typeof(RulesContainer<>));
            serviceCollection.AddAutoDiscoveredRules();

            // TODO: Use service discovery to resolve address from service name.
            // var serviceDiscovery = serviceCollection.GetRegisteredInstance<IServiceDiscovery>();
            var username = this.Configuration.GetSection("RabbitMQ")?["Username"] ?? "queuedemo";
            var password = this.Configuration.GetSection("RabbitMQ")?["Password"] ?? "queuedemo";
            var messageQueueAddress = this.Configuration.GetSection("RabbitMQ")?["Address"] ?? "rabbitmq://localhost";

            serviceCollection.AddMassTransit(
                serviceCollectionConfigurator =>
                    {
                        serviceCollectionConfigurator.AddAuthorizationRequestConsumers();

                        Log.Information(
                            "Connecting to message queue server with address '{ServiceAddress}'",
                            messageQueueAddress);

                        serviceCollectionConfigurator.AddBus(
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
                                                    cfg.ReceiveEndpoint(
                                                        messagesType.GetFlatName(),
                                                        e =>
                                                            {
                                                                e.PrefetchCount = 16;
                                                                e.UseMessageRetry(x => x.Interval(2, 100));
                                                                e.ConfigureConsumer(context, consumerType);
                                                            });
                                                });
                                    }));
                    });

            serviceCollection.AddHostedService<BusHostedService>();
        }
    }
}