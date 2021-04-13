// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationRequestMessageConsumer.cs" company="Stone Assemblies">
// Copyright � 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Server.Services
{
    using System;
    using System.Threading.Tasks;

    using MassTransit;

    using Serilog;

    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Server.Services.Interfaces;

    /// <summary>
    ///     The authorization request message consumer.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public class AuthorizationRequestMessageConsumer<TMessage> : IConsumer<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// The rules container.
        /// </summary>
        private readonly IRulesContainer<TMessage> rulesContainer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthorizationRequestMessageConsumer{TMessage}" /> class.
        /// </summary>
        /// <param name="rulesContainer">
        ///     The rules.
        /// </param>
        public AuthorizationRequestMessageConsumer(IRulesContainer<TMessage> rulesContainer)
        {
            this.rulesContainer = rulesContainer;
        }

        /// <summary>
        ///     Called when a message is ready to be consumed.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var message = new AuthorizationResponseMessage
                              {
                                  IsAuthorized = true,
                              };

            var rules = this.rulesContainer.Rules;
            foreach (var rule in rules)
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
                    message.IsAuthorized = false;
                    break;
                }
            }

            await context.RespondAsync(message);
        }
    }
}