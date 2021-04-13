// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericAuthorizeControllerRouteConvention.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Proxy.Services
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

    /// <summary>
    ///     The generic controller route convention.
    /// </summary>
    public class GenericAuthorizeControllerRouteConvention : IControllerModelConvention
    {
        /// <summary>
        ///     Applies the route convention.
        /// </summary>
        /// <param name="controllerModel">
        ///     The controller model.
        /// </param>
        public void Apply(ControllerModel controllerModel)
        {
            if (controllerModel.ControllerType.IsGenericType)
            {
                var messageType = controllerModel.ControllerType.GenericTypeArguments[0];
                var routeTemplateProvider = new RouteAttribute($"api/authorize/{messageType.Name}");
                controllerModel.Selectors.Add(
                    new SelectorModel
                        {
                            AttributeRouteModel = new AttributeRouteModel(routeTemplateProvider),
                        });
            }
        }
    }
}