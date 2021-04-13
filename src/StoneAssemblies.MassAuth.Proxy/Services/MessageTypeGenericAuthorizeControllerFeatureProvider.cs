// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageTypeGenericAuthorizeControllerFeatureProvider.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Proxy.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Mvc.Controllers;

    using StoneAssemblies.Extensibility.Services.Interfaces;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Proxy.Controllers;

    /// <summary>
    ///     The generic type controller feature provider.
    /// </summary>
    public class MessageTypeGenericAuthorizeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        /// <summary>
        ///     The extension manager.
        /// </summary>
        private readonly IExtensionManager extensionManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageTypeGenericAuthorizeControllerFeatureProvider" /> class.
        /// </summary>
        /// <param name="extensionManager">
        ///     The extension manager.
        /// </param>
        public MessageTypeGenericAuthorizeControllerFeatureProvider(IExtensionManager extensionManager)
        {
            this.extensionManager = extensionManager;
        }

        /// <summary>
        ///     Populates feature.
        /// </summary>
        /// <param name="applicationParts">
        ///     The application parts.
        /// </param>
        /// <param name="controllerFeature">
        ///     The controller feature.
        /// </param>
        public void PopulateFeature(IEnumerable<ApplicationPart> applicationParts, ControllerFeature controllerFeature)
        {
            var extensionAssemblies = this.extensionManager.GetExtensionAssemblies();
            foreach (var extensionAssembly in extensionAssemblies)
            {
                var messageTypes = extensionAssembly.GetTypes()
                    .Where(type => typeof(MessageBase).IsAssignableFrom(type)).ToList();

                foreach (var messageType in messageTypes)
                {
                    var controllerType = typeof(AuthorizeController<>).MakeGenericType(messageType);
                    controllerFeature.Controllers.Add(controllerType.GetTypeInfo());
                }
            }
        }
    }
}