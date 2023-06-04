// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;
    using MassTransit.Configuration;

    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.MassAuth.Services;
    using StoneAssemblies.MassAuth.Services.Interfaces;

    /// <summary>
    ///     The service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     The dynamic buses assemblies.
        /// </summary>
        private static readonly ConcurrentDictionary<IServiceCollection, DynamicBusesAssembly> DynamicBusesAssemblies =
            new ConcurrentDictionary<IServiceCollection, DynamicBusesAssembly>();

        /// <summary>
        ///     The add bus selector.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="predicate">
        ///     The predicate.
        /// </param>
        public static void AddBusSelector<TMessage>(this IServiceCollection serviceCollection, Func<IBus, TMessage, Task<bool>> predicate)
            where TMessage : class
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            serviceCollection.AddSingleton<IBusSelectorPredicate<TMessage>>(new BusSelectorPredicate<TMessage>(predicate));
            serviceCollection.AddSingleton<PredicateBasedBusSelector<TMessage>>();
            serviceCollection.AddBusSelector(provider => provider.GetService<PredicateBasedBusSelector<TMessage>>());
        }

        /// <summary>
        /// Adds bus selector.
        /// </summary>
        /// <param name="serviceCollection">
        /// The service collection.
        /// </param>
        /// <param name="busSelector">
        /// The bus selector.
        /// </param>
        /// <typeparam name="TMessage">
        /// The message type.
        /// </typeparam>
        public static void AddBusSelector<TMessage>(this IServiceCollection serviceCollection, Func<IServiceProvider, IBusSelector<TMessage>> busSelector = null)
            where TMessage : class
        {
            var dynamicBusesAssembly = DynamicBusesAssemblies.GetOrAdd(serviceCollection, sc => new DynamicBusesAssembly());
            dynamicBusesAssembly.RegisterBusesIfRequired(serviceCollection);

            if (busSelector != null)
            {
                serviceCollection.AddSingleton(busSelector);
            }
            else
            {
                serviceCollection.AddSingleton<IBusSelector<TMessage>, DefaultBusSelector<TMessage>>();
            }

            serviceCollection.AddSingleton<IBusSelector>(provider => provider.GetService<IBusSelector<TMessage>>());
        }

        /// <summary>
        /// Adds bus selector.
        /// </summary>
        /// <param name="serviceCollection">
        /// The service collection.
        /// </param>
        /// <param name="messageType">
        ///  The message type
        /// </param>
        public static void AddBusSelector(this IServiceCollection serviceCollection, Type messageType)
        {
            var busSelectorType = typeof(IBusSelector<>).MakeGenericType(messageType);
            var expectedSecondParameterType =
                typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), busSelectorType);

            var type = typeof(ServiceCollectionExtensions);
            var methodInfo = type.GetMethods(BindingFlags.Static | BindingFlags.Public).Select(
                info => info.Name == nameof(AddBusSelector) && info.IsGenericMethod && info.GetParameters().Length == 2
                        && info.TryMakeGenericMethod(out var genericMethod, messageType)
                            ? genericMethod
                            : null)
                .First(genericMethodInfo => genericMethodInfo != null && genericMethodInfo.GetParameters()[1].ParameterType == expectedSecondParameterType);

            methodInfo.Invoke(type, new object[] { serviceCollection, null });
        }

        /// <summary>
        ///     The add mass transit.
        /// </summary>
        /// <param name="collection">
        ///     The collection.
        /// </param>
        /// <param name="typeName">
        ///     The type name.
        /// </param>
        /// <param name="configure">
        ///     The configure.
        /// </param>
        /// <returns>
        ///     The <see cref="IServiceCollection" />.
        /// </returns>
        public static IServiceCollection AddMassTransit(
            this IServiceCollection collection, string typeName, Action<IBusRegistrationConfigurator> configure = null)
        {
            var dynamicBusesAssembly = DynamicBusesAssemblies.GetOrAdd(collection, sc => new DynamicBusesAssembly());

            dynamicBusesAssembly.Configures[typeName] = configure;

            var typeBuilder = dynamicBusesAssembly.ModuleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Interface);
            typeBuilder.AddInterfaceImplementation(typeof(IBus));
            var busType = typeBuilder.CreateType();
            var addMassTransitMethod = typeof(DependencyInjectionRegistrationExtensions).GetMethods().FirstOrDefault(
                info => info.Name == nameof(DependencyInjectionRegistrationExtensions.AddMassTransit)
                        && info.GetGenericArguments().Length == 1);

            if (busType != null && addMassTransitMethod != null)
            {
                dynamicBusesAssembly.BusTypes[typeName] = busType;
                var addMassTransitGenericMethod = addMassTransitMethod.MakeGenericMethod(busType);
                var serviceCollectionBusConfiguratorBusType =
                    typeof(IBusRegistrationConfigurator<>)
                        .MakeGenericType(busType);

                var serviceCollectionBusConfiguratorBusTypeActionType =
                    typeof(Action<>).MakeGenericType(serviceCollectionBusConfiguratorBusType);
                var forwardMethod = typeof(ServiceCollectionExtensions).GetMethod(
                    nameof(Forward),
                    BindingFlags.Static | BindingFlags.NonPublic)?.MakeGenericMethod(serviceCollectionBusConfiguratorBusType);

                if (forwardMethod != null)
                {
                    
                    var forwardMethodDelegate = Delegate.CreateDelegate(
                        serviceCollectionBusConfiguratorBusTypeActionType,
                        forwardMethod);
                    var parameters = new object[]
                                         {
                                             collection, forwardMethodDelegate
                                         };
                    addMassTransitGenericMethod.Invoke(typeof(DependencyInjectionRegistrationExtensions), parameters);
                }
            }

            return collection;
        }

        /// <summary>
        ///     The forward.
        /// </summary>
        /// <param name="configure">
        ///     The configure.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        private static void Forward<T>(T configure)
        {
            IServiceCollection serviceCollection = null;
            if (configure is RegistrationConfigurator registrationConfigurator)
            {
                serviceCollection = registrationConfigurator.GetServiceCollection();
            }

            if (configure is IBusRegistrationConfigurator serviceCollectionBusConfigurator)
            {
                var genericArguments = configure.GetType().GetGenericArguments();
                var dynamicBusesAssembly = DynamicBusesAssemblies[serviceCollection];
                var action = dynamicBusesAssembly.Configures[genericArguments[0].Name];
                action?.Invoke(serviceCollectionBusConfigurator);
            }
        }

        /// <summary>
        ///     The dynamic buses assembly.
        /// </summary>
        private class DynamicBusesAssembly
        {
            /// <summary>
            ///     The stone assemblies dynamic buses.
            /// </summary>
            private const string StoneAssembliesDynamicBuses = "StoneAssemblies.DynamicBuses";

            /// <summary>
            ///     The count.
            /// </summary>
            private static int count;

            /// <summary>
            ///     Initializes a new instance of the <see cref="DynamicBusesAssembly" /> class.
            /// </summary>
            public DynamicBusesAssembly()
            {
                var assemblyName = new AssemblyName($"{StoneAssembliesDynamicBuses}{Interlocked.Increment(ref count)}");
                this.AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                this.ModuleBuilder = this.AssemblyBuilder.DefineDynamicModule($"{assemblyName.Name}.dll");
            }

            /// <summary>
            ///     Gets the bus types.
            /// </summary>
            public Dictionary<string, Type> BusTypes { get; } = new Dictionary<string, Type>();

            /// <summary>
            ///     Gets the configures.
            /// </summary>
            public Dictionary<string, Action<IBusRegistrationConfigurator>> Configures { get; } =
                new Dictionary<string, Action<IBusRegistrationConfigurator>>();

            /// <summary>
            ///     Gets the module builder.
            /// </summary>
            public ModuleBuilder ModuleBuilder { get; }

            /// <summary>
            ///     Gets the assembly builder.
            /// </summary>
            private AssemblyBuilder AssemblyBuilder { get; }

            /// <summary>
            /// The register buses.
            /// </summary>
            /// <param name="serviceCollection">
            /// The service collection.
            /// </param>
            public void RegisterBusesIfRequired(IServiceCollection serviceCollection)
            {
                foreach (var busTypesValue in this.BusTypes.Values)
                {
                    var serviceDescriptor = serviceCollection.FirstOrDefault(descriptor => descriptor.ServiceType == busTypesValue);
                    if (serviceDescriptor != null && serviceCollection.FirstOrDefault(
                            descriptor => descriptor.ServiceType == typeof(IBus)
                                          && descriptor.ImplementationFactory == serviceDescriptor.ImplementationFactory) == null)
                    {
                        serviceCollection.AddSingleton(typeof(IBus), serviceDescriptor.ImplementationFactory);
                    }
                }
            }
        }
    }

    public interface ISuperBus : IBus
    {
    }
}