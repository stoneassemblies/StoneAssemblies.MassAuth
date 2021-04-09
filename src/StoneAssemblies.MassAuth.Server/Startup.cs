namespace StoneAssemblies.MassAuth.Engine
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

    using StoneAssemblies.MassAuth.Engine.Extensions;
    using StoneAssemblies.MassAuth.Engine.Services;
    using StoneAssemblies.MassAuth.Hosting.Extensions;
    using StoneAssemblies.MassAuth.Hosting.Services;
    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Messages.Extensions;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHealthChecks();
            serviceCollection.AddServiceDiscovery();

            serviceCollection.AddExtensionManager(this.Configuration, true);
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