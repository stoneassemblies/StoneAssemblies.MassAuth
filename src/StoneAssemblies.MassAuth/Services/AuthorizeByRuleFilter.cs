namespace StoneAssemblies.MassAuth.Services
{
    using System.Linq;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using Serilog;

    using StoneAssemblies.MassAuth.Extensions;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Services.Extensions;

    public class AuthorizeByRuleFilter : IAsyncActionFilter
    {
        private readonly IClientFactory _clientFactory;

        public AuthorizeByRuleFilter(IClientFactory clientFactory)
        {
            this._clientFactory = clientFactory;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var message = context.ActionArguments.Values.OfType<MessageBase>().FirstOrDefault();
            if (message != null)
            {
                var authorizationMessage = AuthorizationRequestMessageFactory.From(message);

                var clientRequest = this._clientFactory.CreateRequestClient(authorizationMessage.GetType());

                authorizationMessage.Username = context.HttpContext.User.GetUserId();
                authorizationMessage.Claims = context.HttpContext.User.Claims;

                // TODO: Add all useful information in HttpContext 
                // context.HttpContext.Connection

                // TODO: Remove this later, for debug purpose only.
                foreach (var userClaim in context.HttpContext.User.Claims)
                {
                    Log.Information("{ClaimType}  {ClaimValue}", userClaim.Type, userClaim.Value);
                }

                Log.Information(
                    "Sending authorization request message for user {Username}",
                    authorizationMessage.Username);

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