﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Bank.Balance.Services
{
    using System;
    using System.Net.Http;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Logging;

    using StoneAssemblies.Hosting.Services;
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Extensions;
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">
        ///     The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            var serviceDiscovery = ServiceDiscoveryFactory.GetServiceDiscovery();
            services.AddSingleton(serviceDiscovery);

            services.AddMassAuth(
                options =>
                    {
                        options.ReturnForbiddanceReason = true;
                    });

            // TODO: Use service discovery to resolve address from service name.
            // var serviceDiscovery = serviceCollection.GetRegisteredInstance<IServiceDiscovery>();
            var username = this.Configuration.GetSection("RabbitMQ")?["Username"] ?? "queuedemo";
            var password = this.Configuration.GetSection("RabbitMQ")?["Password"] ?? "queuedemo";
            var messageQueueAddress = this.Configuration.GetSection("RabbitMQ")?["Address"] ?? "rabbitmq://localhost:6002";

            //// SingleBus
            services.AddMassTransit(sc => AddBus(sc, messageQueueAddress, string.Empty, username, password));

            // services.AddBusSelector<AccountBalanceRequestMessage>();
            // MultiBus
            //services.AddMassTransit("Bank0Bus", sc => AddBus(sc, messageQueueAddress, "Bank0", username, password));
            //services.AddMassTransit("Bank1Bus", sc => AddBus(sc, messageQueueAddress, "Bank1", username, password));
            //services.AddBusSelector<AccountBalanceRequestMessage>(
            //   (bus, message) =>
            //       {
            //           var parts = bus.Address.AbsolutePath.Split('/');

            //           var virtualHost = parts[1];
            //           if (message.PrimaryAccountNumber.StartsWith("0") && virtualHost.Equals(
            //                   "Bank0",
            //                   StringComparison.InvariantCultureIgnoreCase))
            //           {
            //               return Task.FromResult(true);
            //           }

            //           if (message.PrimaryAccountNumber.StartsWith("1") && virtualHost.Equals(
            //                   "Bank1",
            //                   StringComparison.InvariantCultureIgnoreCase))
            //           {
            //               return Task.FromResult(true);
            //           }

            //           return Task.FromResult(false);
            //       });

            services.AddControllers();
        }

        private static void AddBus(
            IBusRegistrationConfigurator sc, string messageQueueAddress, string virtualHost, string username, string password)
        {
            sc.AddBus(
                context =>
                    {
                        var busControl = Bus.Factory.CreateUsingRabbitMq(
                            cfg =>
                                {
                                    cfg.Host(
                                        new Uri(new Uri(messageQueueAddress), new Uri(virtualHost, UriKind.Relative)),
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
                                });

                        return busControl;
                    });

            sc.AddDefaultAuthorizationRequestClient<AccountBalanceRequestMessage>();
        }
    }

}