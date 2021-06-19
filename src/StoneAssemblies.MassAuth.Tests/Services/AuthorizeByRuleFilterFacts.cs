﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeByRuleFilterFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;
    using MassTransit.Clients;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    using Moq;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Services;

    using Xunit;

    /// <summary>
    ///     The authorize by rule filter facts.
    /// </summary>
    public class AuthorizeByRuleFilterFacts
    {
        /// <summary>
        ///     The the_ on action execution async_ method.
        /// </summary>
        public class The_OnActionExecutionAsync_Method
        {
            /// <summary>
            ///     Does not invoke next action delegate when the request is unauthorized.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Does_Not_Invoke_Next_Action_Delegate_When_The_Request_Is_Unauthorized()
            {
                var actionContext = new ActionContext
                                        {
                                            HttpContext = new DefaultHttpContext(),
                                            RouteData = new RouteData(),
                                            ActionDescriptor = new ActionDescriptor()
                                        };
                var actionArguments = new Dictionary<string, object>
                                          {
                                              {
                                                  "accountBalanceRequestMessage", new AccountBalanceRequestMessage
                                                      {
                                                          PrimaryAccountNumber = "123456789012"
                                                      }
                                              }
                                          };

                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    actionArguments,
                    new Mock<Controller>());

                var clientFactoryMock = new Mock<IClientFactory>();
                var requestClientMock =
                    new Mock<IRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
                requestClientMock.Setup(
                    client => client.GetResponse<AuthorizationResponseMessage>(
                        It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(),
                        CancellationToken.None,
                        default)).ReturnsAsync(
                    () =>
                        {
                            var mock = new Mock<ConsumeContext<AuthorizationResponseMessage>>();
                            mock.Setup(context => context.Message).Returns(
                                new AuthorizationResponseMessage
                                    {
                                        IsAuthorized = false
                                    });
                            var messageResponse = new MessageResponse<AuthorizationResponseMessage>(mock.Object);
                            return messageResponse;
                        });

                clientFactoryMock
                    .Setup(
                        factory => factory
                            .CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                                It.IsAny<RequestTimeout>())).Returns(requestClientMock.Object);

                var nextInvoked = false;
                var authorizeByRuleFilter = new AuthorizeByRuleFilter(clientFactoryMock.Object);
                var actionExecutionDelegate = new ActionExecutionDelegate(
                    () =>
                        {
                            nextInvoked = true;
                            return Task.FromResult<ActionExecutedContext>(null);
                        });

                await authorizeByRuleFilter.OnActionExecutionAsync(actionExecutingContext, actionExecutionDelegate);

                Assert.False(nextInvoked);
            }

            /// <summary>
            ///     The invokes next action delegate when the request have no message as parameter.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Invokes_Next_Action_Delegate_When_The_Request_Have_No_Message_As_Parameter()
            {
                var actionContext = new ActionContext
                                        {
                                            HttpContext = new DefaultHttpContext(),
                                            RouteData = new RouteData(),
                                            ActionDescriptor = new ActionDescriptor()
                                        };
                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    new Dictionary<string, object>(),
                    new Mock<Controller>().Object);

                var clientFactoryMock = new Mock<IClientFactory>();

                var nextInvoked = false;
                var authorizeByRuleFilter = new AuthorizeByRuleFilter(clientFactoryMock.Object);
                var actionExecutionDelegate = new ActionExecutionDelegate(
                    () =>
                        {
                            nextInvoked = true;
                            return Task.FromResult<ActionExecutedContext>(null);
                        });
                await authorizeByRuleFilter.OnActionExecutionAsync(actionExecutingContext, actionExecutionDelegate);

                Assert.True(nextInvoked);
            }

            /// <summary>
            ///     Invokes next action delegate when the request is authorized.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Invokes_Next_Action_Delegate_When_The_Request_Is_Authorized()
            {
                var actionContext = new ActionContext
                                        {
                                            HttpContext = new DefaultHttpContext(),
                                            RouteData = new RouteData(),
                                            ActionDescriptor = new ActionDescriptor()
                                        };

                var actionArguments = new Dictionary<string, object>
                                          {
                                              {
                                                  "accountBalanceRequestMessage", new AccountBalanceRequestMessage
                                                      {
                                                          PrimaryAccountNumber = "123456789012"
                                                      }
                                              }
                                          };

                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    actionArguments,
                    new Mock<Controller>());

                var clientFactoryMock = new Mock<IClientFactory>();
                var requestClientMock =
                    new Mock<IRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
                requestClientMock.Setup(
                    client => client.GetResponse<AuthorizationResponseMessage>(
                        It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(),
                        CancellationToken.None,
                        default)).ReturnsAsync(
                    () =>
                        {
                            var mock = new Mock<ConsumeContext<AuthorizationResponseMessage>>();
                            mock.Setup(context => context.Message).Returns(
                                new AuthorizationResponseMessage
                                    {
                                        IsAuthorized = true
                                    });
                            var messageResponse = new MessageResponse<AuthorizationResponseMessage>(mock.Object);
                            return messageResponse;
                        });

                clientFactoryMock
                    .Setup(
                        factory => factory
                            .CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                                It.IsAny<RequestTimeout>())).Returns(requestClientMock.Object);

                var nextInvoked = false;
                var authorizeByRuleFilter = new AuthorizeByRuleFilter(clientFactoryMock.Object);
                var actionExecutionDelegate = new ActionExecutionDelegate(
                    () =>
                        {
                            nextInvoked = true;
                            return Task.FromResult<ActionExecutedContext>(null);
                        });

                await authorizeByRuleFilter.OnActionExecutionAsync(actionExecutingContext, actionExecutionDelegate);

                Assert.True(nextInvoked);
            }
        }
    }
}