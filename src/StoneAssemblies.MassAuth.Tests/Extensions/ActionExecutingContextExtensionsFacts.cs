// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionExecutingContextExtensionsFacts.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Extensions
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    using Moq;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Extensions;
    using StoneAssemblies.MassAuth.Services.Attributes;

    using Xunit;

    public class ActionExecutingContextExtensionsFacts
    {
        /// <summary>
        ///     The get messages method.
        /// </summary>
        public class The_GetMessages_Method
        {
            /// <summary>
            ///     The controller method.
            /// </summary>
            /// <param name="primaryAccountNumber">
            ///     The primary account number.
            /// </param>
            [AuthorizeByRule(typeof(AccountBalanceRequestMessage))]
            public void ControllerMethod(string primaryAccountNumber)
            {
            }

            /// <summary>
            ///     The controller method 2.
            /// </summary>
            /// <param name="primaryAccountNumber">
            ///     The primary account number.
            /// </param>
            [AuthorizeByRule]
            public void ControllerMethod2(string primaryAccountNumber)
            {
            }

            /// <summary>
            ///     Does not return messages from action attribute when messages types are not available.
            /// </summary>
            [Fact]
            public void Does_Not_Return_Messages_From_Action_Attribute_When_MessagesTypes_Are_Not_Available()
            {
                var actionContext = new ActionContext
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData(),
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        MethodInfo = this.GetType()
                                                                           .GetMethod(nameof(this.ControllerMethod2)),
                    },
                };
                var actionArguments = new Dictionary<string, object>
                                          {
                                              {
                                                  "PrimaryAccountNumber", "123456789012"
                                              },
                                              {
                                                  "UserName", "Jane"
                                              },
                                          };

                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    actionArguments,
                    new Mock<Controller>());

                var messages = actionExecutingContext.GetMessages();

                Assert.Empty(messages);
            }

            /// <summary>
            ///     Returns messages from action attribute.
            /// </summary>
            [Fact]
            public void Returns_Messages_From_Action_Attribute()
            {
                var actionContext = new ActionContext
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData(),
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        MethodInfo = this.GetType()
                                                                           .GetMethod(nameof(this.ControllerMethod)),
                    },
                };
                var actionArguments = new Dictionary<string, object>
                                          {
                                              {
                                                  "PrimaryAccountNumber", "123456789012"
                                              },
                                          };

                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    actionArguments,
                    new Mock<Controller>());

                var messages = actionExecutingContext.GetMessages();

                Assert.NotEmpty(messages);
            }

            /// <summary>
            ///     Returns messages from action attribute ignoring the unmatched arguments.
            /// </summary>
            [Fact]
            public void Returns_Messages_From_Action_Attribute_Ignoring_The_Unmatched_Arguments()
            {
                var actionContext = new ActionContext
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData(),
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        MethodInfo = this.GetType()
                                                                           .GetMethod(nameof(this.ControllerMethod)),
                    },
                };
                var actionArguments = new Dictionary<string, object>
                                          {
                                              {
                                                  "PrimaryAccountNumber", "123456789012"
                                              },
                                              {
                                                  "UserName", "Jane"
                                              },
                                          };

                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    actionArguments,
                    new Mock<Controller>());

                var messages = actionExecutingContext.GetMessages();

                Assert.NotEmpty(messages);
            }

            /// <summary>
            ///     Returns messages from action message base arguments.
            /// </summary>
            [Fact]
            public void Returns_Messages_From_Action_MessageBase_Arguments()
            {
                var actionContext = new ActionContext
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor(),
                };
                var actionArguments = new Dictionary<string, object>
                                          {
                                              {
                                                  "accountBalanceRequestMessage", new AccountBalanceRequestMessage
                                                                                      {
                                                                                          PrimaryAccountNumber = "123456789012",
                                                                                      }
                                              },
                                          };

                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    actionArguments,
                    new Mock<Controller>());

                var messages = actionExecutingContext.GetMessages();

                Assert.NotEmpty(messages);
            }
        }
    }
}