#nullable enable

namespace StoneAssemblies.MassAuth.Server.Extensions
{
    using System;
    using System.Text.Json.Serialization;

    using MassTransit;

    using Serilog;

    using StoneAssemblies.MassAuth.Hosting.Extensions;

    public static class ServiceCollectionConfiguratorExtensions
    {
        public static void AddBus(
            this IBusRegistrationConfigurator busRegistrationConfigurator, string messageQueueAddress, string? virtualHost,
            string username, string password)
        {
            busRegistrationConfigurator.AddAuthorizationRequestConsumers();

            Log.Information("Connecting to message queue server with address '{ServiceAddress}'", messageQueueAddress);

            busRegistrationConfigurator.AddBus(
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

                        cfg.ConfigureJsonSerializerOptions(
                            options =>
                            {
                                options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                                return options;
                            });

                        busRegistrationConfigurator.ConfigureAuthorizationRequestConsumers(
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