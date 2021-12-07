// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeByRuleFilterFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Dasync.Collections;

    using MassTransit;
    using MassTransit.Clients;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    using Moq;

    using StoneAssemblies.Contrib.MassTransit.Services.Interfaces;
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Services;
    using StoneAssemblies.MassAuth.Services.Attributes;
    using StoneAssemblies.MassAuth.Services.Extensions;
    using StoneAssemblies.MassAuth.Services.Options;

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
                var requestClientMock = new Mock<IRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
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
                        factory => factory.CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                            It.IsAny<RequestTimeout>())).Returns(requestClientMock.Object);

                var nextInvoked = false;
                var busSelectorMock = new Mock<IBusSelector<AccountBalanceRequestMessage>>();
                busSelectorMock.Setup(selector => selector.SelectClientFactories(It.IsAny<object>())).Returns(
                (object @object) => ToAsyncEnumerable(clientFactoryMock.Object));
                var authorizeByRuleFilter = new AuthorizeByRuleFilter(new AuthorizeByRuleFilterConfigurationOptions(),
                    new List<IBusSelector>
                        {
                            busSelectorMock.Object
                        }) as IAsyncActionFilter;
                var actionExecutionDelegate = new ActionExecutionDelegate(
                    () =>
                        {
                            nextInvoked = true;
                            return Task.FromResult<ActionExecutedContext>(null);
                        });

                await authorizeByRuleFilter.OnActionExecutionAsync(actionExecutingContext, actionExecutionDelegate);

                Assert.False(nextInvoked);
            }

            [Fact]
            public async Task Does_Not_Invoke_Next_Action_Delegate_When_The_Request_Is_Unauthorized_Building_The_Message_From_The_Attribute()
            {
                var actionContext = new ActionContext
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData(),
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        MethodInfo = this.GetType().GetMethod(nameof(ControllerMethod))
                    }
                };
                var actionArguments = new Dictionary<string, object>
                                          {
                                              {
                                                  "primaryAccountNumber", "123456789012"
                                              }
                                          };

                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    actionArguments,
                    new Mock<Controller>());

                var clientFactoryMock = new Mock<IClientFactory>();
                var requestClientMock = new Mock<IRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
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
                        factory => factory.CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                            It.IsAny<RequestTimeout>())).Returns(requestClientMock.Object);

                var nextInvoked = false;
                var busSelectorMock = new Mock<IBusSelector<AccountBalanceRequestMessage>>();
                busSelectorMock.Setup(selector => selector.SelectClientFactories(It.IsAny<object>())).Returns(
                (object @object) => ToAsyncEnumerable(clientFactoryMock.Object));
                var authorizeByRuleFilter = new AuthorizeByRuleFilter(new AuthorizeByRuleFilterConfigurationOptions(),
                    new List<IBusSelector>
                        {
                            busSelectorMock.Object
                        }) as IAsyncActionFilter;
                var actionExecutionDelegate = new ActionExecutionDelegate(
                    () =>
                        {
                            nextInvoked = true;
                            return Task.FromResult<ActionExecutedContext>(null);
                        });

                await authorizeByRuleFilter.OnActionExecutionAsync(actionExecutingContext, actionExecutionDelegate);

                Assert.False(nextInvoked);
            }


            [Fact]
            public async Task Does_Not_Invoke_Next_Action_Delegate_When_The_Request_Is_Unauthorized_And_ReturnForbiddanceReason_Is_True()
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
                var requestClientMock = new Mock<IRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
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
                                    IsAuthorized = false,
                                    ForbiddanceReason = "The Forbiddance Reason"
                                });
                            var messageResponse = new MessageResponse<AuthorizationResponseMessage>(mock.Object);
                            return messageResponse;
                        });

                clientFactoryMock
                    .Setup(
                        factory => factory.CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                            It.IsAny<RequestTimeout>())).Returns(requestClientMock.Object);

                var nextInvoked = false;
                var busSelectorMock = new Mock<IBusSelector<AccountBalanceRequestMessage>>();
                busSelectorMock.Setup(selector => selector.SelectClientFactories(It.IsAny<object>())).Returns(
                    (object @object) => ToAsyncEnumerable(clientFactoryMock.Object));
                var authorizeByRuleFilter = new AuthorizeByRuleFilter(
                                                new AuthorizeByRuleFilterConfigurationOptions()
                                                {
                                                    ReturnForbiddanceReason = true
                                                },
                                                new List<IBusSelector>
                                                    {
                                                        busSelectorMock.Object
                                                    }) as IAsyncActionFilter;
                var actionExecutionDelegate = new ActionExecutionDelegate(
                    () =>
                        {
                            nextInvoked = true;
                            return Task.FromResult<ActionExecutedContext>(null);
                        });

                await authorizeByRuleFilter.OnActionExecutionAsync(actionExecutingContext, actionExecutionDelegate);

                Assert.False(nextInvoked);
            }
            [Fact]
            public async Task Does_Not_Invoke_Next_Action_Delegate_When_The_Request_Is_Unauthorized_And_ReturnForbiddanceReason_Is_True_Building_The_Message_From_The_Attribute()
            {
                var actionContext = new ActionContext
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData(),
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        MethodInfo = this.GetType().GetMethod(nameof(this.ControllerMethod))
                    }
                };
                var actionArguments = new Dictionary<string, object>
                                          {
                                              {
                                                  "primaryAccountNumber", "123456789012"
                                              }
                                          };

                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    actionArguments,
                    new Mock<Controller>());

                var clientFactoryMock = new Mock<IClientFactory>();
                var requestClientMock = new Mock<IRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
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
                                    IsAuthorized = false,
                                    ForbiddanceReason = "The Forbiddance Reason"
                                });
                            var messageResponse = new MessageResponse<AuthorizationResponseMessage>(mock.Object);
                            return messageResponse;
                        });

                clientFactoryMock
                    .Setup(
                        factory => factory.CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                            It.IsAny<RequestTimeout>())).Returns(requestClientMock.Object);

                var nextInvoked = false;
                var busSelectorMock = new Mock<IBusSelector<AccountBalanceRequestMessage>>();
                busSelectorMock.Setup(selector => selector.SelectClientFactories(It.IsAny<object>())).Returns(
                    (object @object) => ToAsyncEnumerable(clientFactoryMock.Object));
                var authorizeByRuleFilter = new AuthorizeByRuleFilter(
                                                new AuthorizeByRuleFilterConfigurationOptions()
                                                {
                                                    ReturnForbiddanceReason = true
                                                },
                                                new List<IBusSelector>
                                                    {
                                                        busSelectorMock.Object
                                                    }) as IAsyncActionFilter;
                var actionExecutionDelegate = new ActionExecutionDelegate(
                    () =>
                        {
                            nextInvoked = true;
                            return Task.FromResult<ActionExecutedContext>(null);
                        });

                await authorizeByRuleFilter.OnActionExecutionAsync(actionExecutingContext, actionExecutionDelegate);

                Assert.False(nextInvoked);
            }

            [Fact]
            public async Task Does_Not_Invoke_Next_Action_Delegate_When_The_Request_Throws_An_Exception()
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
                var requestClientMock = new Mock<IRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
                requestClientMock.Setup(
                    client => client.GetResponse<AuthorizationResponseMessage>(
                        It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(),
                        CancellationToken.None,
                        default)).ThrowsAsync(new Exception());

                clientFactoryMock
                    .Setup(
                        factory => factory.CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                            It.IsAny<RequestTimeout>())).Returns(requestClientMock.Object);

                var nextInvoked = false;
                var busSelectorMock = new Mock<IBusSelector<AccountBalanceRequestMessage>>();
                busSelectorMock.Setup(selector => selector.SelectClientFactories(It.IsAny<object>())).Returns(
                    (object @object) => ToAsyncEnumerable(clientFactoryMock.Object));
                var authorizeByRuleFilter = new AuthorizeByRuleFilter(
                                                new AuthorizeByRuleFilterConfigurationOptions()
                                                {
                                                    ReturnForbiddanceReason = true
                                                },
                                                new List<IBusSelector>
                                                    {
                                                        busSelectorMock.Object
                                                    }) as IAsyncActionFilter;
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
            /// To async enumerable.
            /// </summary>
            /// <param name="object">
            /// The object.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="IAsyncEnumerable"/>.
            /// </returns>
            private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(T @object)
            {
                yield return @object;
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
                var busSelectorMock = new Mock<IBusSelector<AccountBalanceRequestMessage>>();
                busSelectorMock.Setup(selector => selector.SelectClientFactories(It.IsAny<object>())).Returns(
                    (object @object) => ToAsyncEnumerable(clientFactoryMock.Object));
                var authorizeByRuleFilter = new AuthorizeByRuleFilter(
                                                new AuthorizeByRuleFilterConfigurationOptions(),
                                                new List<IBusSelector>
                                                    {
                                                        busSelectorMock.Object
                                                    }) as IAsyncActionFilter;
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

                var clientFactoryMock = new Mock<IClientFactory>();
                var requestClientMock = new Mock<IRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
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
                                    IsAuthorized = true,
                                });
                            var messageResponse = new MessageResponse<AuthorizationResponseMessage>(mock.Object);
                            return messageResponse;
                        });

                clientFactoryMock
                    .Setup(
                        factory => factory.CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                            It.IsAny<RequestTimeout>())).Returns(requestClientMock.Object);

                var nextInvoked = false;

                var busSelectorMock = new Mock<IBusSelector<AccountBalanceRequestMessage>>();
                busSelectorMock.Setup(selector => selector.SelectClientFactories(It.IsAny<object>())).Returns(
                    (object @object) => ToAsyncEnumerable(clientFactoryMock.Object));

                var authorizeByRuleFilter = new AuthorizeByRuleFilter(
                                                new AuthorizeByRuleFilterConfigurationOptions(),
                                                new List<IBusSelector>
                                                    {
                                                        busSelectorMock.Object,
                                                    }) as IAsyncActionFilter;
                var actionExecutionDelegate = new ActionExecutionDelegate(
                    () =>
                        {
                            nextInvoked = true;
                            return Task.FromResult<ActionExecutedContext>(null);
                        });

                await authorizeByRuleFilter.OnActionExecutionAsync(actionExecutingContext, actionExecutionDelegate);

                Assert.True(nextInvoked);
            }

            [AuthorizeByRule(typeof(AccountBalanceRequestMessage))]
            [Theory]
            public void ControllerMethod(string primaryAccountNumber)
            {
            }
        }
    }
}