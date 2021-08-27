// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Bank.Balance.Services
{
    using System.Net.Http;

    using MassTransit;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Logging;

    using Newtonsoft.Json;

    using StoneAssemblies.Hosting.Services;
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Services;
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

            services.AddMassAuth();

            // TODO: Use service discovery to resolve address from service name.
            // var serviceDiscovery = serviceCollection.GetRegisteredInstance<IServiceDiscovery>();
            var username = this.Configuration.GetSection("RabbitMQ")?["Username"] ?? "queuedemo";
            var password = this.Configuration.GetSection("RabbitMQ")?["Password"] ?? "queuedemo";
            var messageQueueAddress = this.Configuration.GetSection("RabbitMQ")?["Address"] ?? "rabbitmq://localhost";

            services.AddMassTransit(
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
                                                    return s;
                                                });
                                    }));

                        sc.AddDefaultAuthorizationRequestClient<AccountBalanceRequestMessage>();
                    });

            var identityServerAuthority = this.Configuration.GetSection("IdentityServer")?["Authority"] ?? "http://172.30.64.1:6005/auth/realms/master";
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    {
                        options.Authority = identityServerAuthority;
                        options.RequireHttpsMetadata = !string.IsNullOrWhiteSpace(identityServerAuthority) && identityServerAuthority.StartsWith("https://");
                        options.Audience = "stoneassemblies-massauth-bank-balance-services";
                        options.BackchannelHttpHandler = new HttpClientHandler
                                                             {
                                                                 ServerCertificateCustomValidationCallback = delegate { return true; }
                                                             };
                    });

            services.AddControllers();
            services.AddHostedService<BusHostedService>();
        }
    }
}
