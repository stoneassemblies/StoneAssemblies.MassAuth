namespace StoneAssemblies.MassAuth.Engine.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MassTransit;

    using Serilog;

    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    public class AuthorizationRequestMessageConsumer<TMessage> : IConsumer<TMessage>
        where TMessage : class
    {
        private readonly List<IRule<TMessage>> rules;

        public AuthorizationRequestMessageConsumer(IEnumerable<IRule<TMessage>> rules)
        {
            this.rules = rules.ToList();
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            foreach (var rule in this.rules.Where(r => r.IsEnabled))
            {
                var succeeded = false;
                try
                {
                    Log.Information("Evaluating rule '{RuleName}'", rule.Name);
                    succeeded = await rule.EvaluateAsync(context.Message);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error evaluating rule '{RuleName}'", rule.Name);
                }

                if (!succeeded)
                {
                    Log.Information("Unauthorized by rule '{RuleName}' ", rule.Name);

                    await context.RespondAsync(new AuthorizationResponseMessage
                                                   {
                                                       IsAuthorized = false
                                                   });
                    return;
                }
            }

            await context.RespondAsync(new AuthorizationResponseMessage
                                           {
                                               IsAuthorized = true
                                           });
        }
    }
}