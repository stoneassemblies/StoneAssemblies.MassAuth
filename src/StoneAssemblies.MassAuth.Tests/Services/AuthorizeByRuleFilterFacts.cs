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

    public class AuthorizeByRuleFilterFacts
    {
        public class The_OnActionExecutionAsync_Method
        {
            [Fact]
            public async Task Calls_Next_Action_When_The_Request_Have_No_Message_As_Parameter()
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
                    Mock.Of<Controller>());

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
            }

            [Fact]
            public async Task Calls_Next_When_The_Request_Is_Authorized()
            {
                var actionContext = new ActionContext
                                        {
                                            HttpContext = new DefaultHttpContext(),
                                            RouteData = new RouteData(),
                                            ActionDescriptor = new ActionDescriptor()
                                        };
                var actionArguments = new Dictionary<string, object>
                                          {
                                              { "accountBalanceRequestMessage", new AccountBalanceRequestMessage() }
                                          };

                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    actionArguments,
                    Mock.Of<Controller>());

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

            [Fact]
            public async Task Does_Not_Call_Next_When_The_Request_Is_Unahuthorized()
            {
                var actionContext = new ActionContext
                                        {
                                            HttpContext = new DefaultHttpContext(),
                                            RouteData = new RouteData(),
                                            ActionDescriptor = new ActionDescriptor()
                                        };
                var actionArguments = new Dictionary<string, object>
                                          {
                                              { "accountBalanceRequestMessage", new AccountBalanceRequestMessage() }
                                          };

                var actionExecutingContext = new ActionExecutingContext(
                    actionContext,
                    new List<IFilterMetadata>(),
                    actionArguments,
                    Mock.Of<Controller>());

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
        }
    }
}