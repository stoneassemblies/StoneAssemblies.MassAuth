// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeByRuleFilter.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using Serilog;

    using StoneAssemblies.Contrib.MassTransit.Services.Interfaces;
    using StoneAssemblies.MassAuth.Extensions;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Services.Extensions;

    /// <summary>
    ///     The authorize by rule filter.
    /// </summary>
    public class AuthorizeByRuleFilter : IAsyncActionFilter
    {
        /// <summary>
        /// The bus selector.
        /// </summary>
        private readonly List<IBusSelector> busSelectors;

        ///// <summary>
        /////     The client factories.
        ///// </summary>
        //private readonly IClientFactory clientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeByRuleFilter"/> class.
        /// </summary>
        public AuthorizeByRuleFilter(IEnumerable<IBusSelector> busSelectors)
        {
            this.busSelectors = busSelectors.ToList();
        }

        /// <summary>
        ///     On action execution async.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="next">
        ///     The next.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var message = context.ActionArguments.Values.OfType<MessageBase>().FirstOrDefault();
            if (message != null)
            {
                var busSelector = this.busSelectors.FirstOrDefault(selector => selector.GetType().GetGenericArguments()[0] == message.GetType());
                if (busSelector != null)
                {
                    var clientFactories = busSelector.SelectClientFactories(message);
                    var authorizationMessage = AuthorizationRequestMessageFactory.From(message);

                    var tasks = new List<Task<Response<AuthorizationResponseMessage>>>();
                    await foreach (var factory in clientFactories)
                    {
                        var clientRequest = factory.CreateRequestClient(authorizationMessage.GetType());

                        authorizationMessage.UserId = context.HttpContext.User.GetUserId();
                        authorizationMessage.Claims = context.HttpContext.User.Claims;
                        try
                        {
                            authorizationMessage.AccessToken = await context.HttpContext.GetTokenAsync("access_token");
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex, "Error getting the access token");
                        }

                        tasks.Add(Task.Run(() => clientRequest.GetResponse<AuthorizationResponseMessage>(authorizationMessage)));
                    }

                    if (tasks.Count == 0)
                    {
                        context.Result = new UnauthorizedResult();
                    }
                    else
                    {
                        var responses = await Task.WhenAll(tasks);
                        if (responses.Any(r => !r.Message.IsAuthorized))
                        {
                            context.Result = new UnauthorizedResult();
                        }
                    }
                }
            }

            if (context.Result == null)
            {
                await next();
            }
        }

    }
}