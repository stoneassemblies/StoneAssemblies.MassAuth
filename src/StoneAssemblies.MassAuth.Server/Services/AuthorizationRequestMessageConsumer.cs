// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationRequestMessageConsumer.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Server.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MassTransit;

    using Serilog;

    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

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
        ///     The rules.
        /// </summary>
        private readonly List<IRule<TMessage>> rules;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthorizationRequestMessageConsumer{TMessage}" /> class.
        /// </summary>
        /// <param name="rules">
        ///     The rules.
        /// </param>
        public AuthorizationRequestMessageConsumer(IEnumerable<IRule<TMessage>> rules)
        {
            this.rules = rules.ToList();
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
            var message = new AuthorizationResponseMessage();
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
                    message.IsAuthorized = false;
                    break;
                }
            }

            await context.RespondAsync(message);
        }
    }
}