// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeByRuleFilter.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using Serilog;

    using StoneAssemblies.MassAuth.Extensions;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Services.Extensions;

    /// <summary>
    ///     The authorize by rule filter.
    /// </summary>
    public class AuthorizeByRuleFilter : IAsyncActionFilter
    {
        /// <summary>
        ///     The client factory.
        /// </summary>
        private readonly IClientFactory clientFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthorizeByRuleFilter" /> class.
        /// </summary>
        /// <param name="clientFactory">
        ///     The client factory.
        /// </param>
        public AuthorizeByRuleFilter(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
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
                var authorizationMessage = AuthorizationRequestMessageFactory.From(message);

                var clientRequest = this.clientFactory.CreateRequestClient(authorizationMessage.GetType());

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

                var response = await clientRequest.GetResponse<AuthorizationResponseMessage>(authorizationMessage);
                if (!response.Message.IsAuthorized)
                {
                    context.Result = new UnauthorizedResult();
                }
            }

            if (context.Result == null)
            {
                await next();
            }
        }
    }
}
