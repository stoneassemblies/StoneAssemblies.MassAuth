// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeByRuleFilter.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Connections.Features;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Server.HttpSys;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using Serilog;

    using StoneAssemblies.MassAuth.Extensions;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Services.Attributes;
    using StoneAssemblies.MassAuth.Services.Extensions;
    using StoneAssemblies.MassAuth.Services.Interfaces;
    using StoneAssemblies.MassAuth.Services.Options;

    /// <summary>
    ///     The authorize by rule filter.
    /// </summary>
    public class AuthorizeByRuleFilter : IAsyncActionFilter, IHubFilter
    {
        private readonly AuthorizeByRuleFilterConfigurationOptions options;

        private readonly IServiceProvider serviceProvider;

        /// <summary>
        ///     The bus selector.
        /// </summary>
        private readonly List<IBusSelector> busSelectors;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthorizeByRuleFilter" /> class.
        /// </summary>
        public AuthorizeByRuleFilter(
            AuthorizeByRuleFilterConfigurationOptions options, IEnumerable<IBusSelector> busSelectors,
            IServiceProvider serviceProvider)
        {
            this.options = options;
            this.serviceProvider = serviceProvider;
            this.busSelectors = busSelectors.ToList();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthorizeByRuleFilter" /> class.
        /// </summary>
        public AuthorizeByRuleFilter(AuthorizeByRuleFilterConfigurationOptions options, IServiceProvider serviceProvider)
            : this(options, Array.Empty<IBusSelector>(), serviceProvider)
        {
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
        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var messages = context.GetMessages();
            var authorizationResult = await this.AuthorizationRequestAsync(context.HttpContext, messages);
            if (authorizationResult.IsAuthorized)
            {
                await next();
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                };

                problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);

                if (this.options.ReturnForbiddanceReason)
                {
                    problemDetails.Detail = JsonSerializer.Serialize(authorizationResult.ForbiddanceReason);
                }

                context.Result = new ObjectResult(problemDetails)
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }

        /// <summary>
        ///     Invoke method async
        /// </summary>
        /// <param name="invocationContext">
        ///     Invocation context.
        /// </param>
        /// <param name="next">
        ///     The next task.
        /// </param>
        /// <returns>
        ///     The current task.
        /// </returns>
        async ValueTask<object> IHubFilter.InvokeMethodAsync(
            HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            var authorizeByRuleAttribute =
                invocationContext.HubMethod.GetCustomAttributes().OfType<AuthorizeByRuleAttribute>().FirstOrDefault();
            var messages = invocationContext.HubMethodArguments.OfType<MessageBase>().ToList();
            var httpContext = invocationContext.Context.Features.Get<IHttpContextFeature>()?.HttpContext;

            if (authorizeByRuleAttribute != null)
            {
                var authorization = await this.AuthorizationRequestAsync(httpContext, messages);
                if (!authorization.IsAuthorized)
                {
                    await invocationContext.Hub.Clients.Caller.SendAsync("Unauthorized");
                    throw new HubException("Unauthorized");
                }
            }

            return await next(invocationContext);
        }

        /// <summary>
        ///     Determines whether all message in the list are authorized.
        /// </summary>
        /// <param name="httpContext">
        ///     The http context.
        /// </param>
        /// <param name="messages">
        ///     The messages
        /// </param>
        /// <returns>
        ///     <c>True</c> whether all messages are authorized, otherwise <c>False</c>
        /// </returns>
        private async Task<AuthorizationResult> AuthorizationRequestAsync(HttpContext httpContext, List<MessageBase> messages)
        {
            var authorizationResult = AuthorizationResult.Authorized();
            for (var idx = 0; idx < messages.Count && authorizationResult.IsAuthorized; idx++)
            {
                authorizationResult = await this.AuthorizationRequestAsync(httpContext, messages[idx]);
            }

            return authorizationResult;
        }

        /// <summary>
        ///     Determines whether all messages in the list are authorized.
        /// </summary>
        /// <param name="httpContext">
        ///     The http context.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     <c>True</c> whether the message is authorized, otherwise <c>False</c>
        /// </returns>
        private async Task<AuthorizationResult> AuthorizationRequestAsync(HttpContext httpContext, MessageBase message)
        {
            var messageType = message.GetType();
            var busSelector = this.busSelectors.FirstOrDefault(selector => typeof(IBusSelector<>).MakeGenericType(messageType).IsInstanceOfType(selector)) ??
                              (IBusSelector)ActivatorUtilities.CreateInstance(this.serviceProvider, typeof(DefaultBusSelector<>).MakeGenericType(messageType));

            var clientFactories = busSelector.SelectClientFactories(message);
            var authorizationMessage = AuthorizationRequestMessageFactory.From(message);

            var tasks = new List<Task<AuthorizationResponseMessage>>();
            await foreach (var factory in clientFactories)
            {
                var clientRequest = factory.CreateRequestClient(authorizationMessage.GetType());
                if (httpContext != null)
                {
                    authorizationMessage.UserId = httpContext.User?.GetUserId();
                    authorizationMessage.Claims = httpContext.User?.Claims;
                    try
                    {
                        authorizationMessage.AccessToken = await httpContext.GetTokenAsync("access_token");
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Error getting the access token");
                    }
                }

                tasks.Add(Task.Run(() => clientRequest.GetAuthorizationResponseMessageAsync(authorizationMessage)));
            }

            if (tasks.Count == 0)
            {
                return AuthorizationResult.Forbidden();
            }

            var messages = await Task.WhenAll(tasks);
            var authorizationResponseMessage = messages.FirstOrDefault(responseMessage => !responseMessage.IsAuthorized);
            if (authorizationResponseMessage != null)
            {
                return AuthorizationResult.Forbidden(authorizationResponseMessage.ForbiddanceReason);
            }

            return AuthorizationResult.Authorized();
        }

    }
}