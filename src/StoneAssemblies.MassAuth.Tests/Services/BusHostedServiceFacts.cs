namespace StoneAssemblies.MassAuth.Tests.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;

    using Moq;

    using StoneAssemblies.MassAuth.Services;

    using Xunit;

    /// <summary>
    ///     The bus hosted service facts.
    /// </summary>
    public class BusHostedServiceFacts
    {
        /// <summary>
        ///     The the_ constructor.
        /// </summary>
        public class The_Constructor
        {
            /// <summary>
            ///     The throws argument null exception  given null reference for bus control parameter.
            /// </summary>
            [Fact]
            public void Throws_ArgumentNullException_Given_Null_Reference_For_BusControl_Parameter()
            {
                Assert.Throws<ArgumentNullException>(() => new BusHostedService(null));
            }
        }

        /// <summary>
        ///     The start async method.
        /// </summary>
        public class The_StartAsync_Method
        {
            /// <summary>
            ///     The calls the start async method of bus control once.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Calls_The_StartAsync_Method_Of_BusControl_Once()
            {
                var busControlMock = new Mock<IBusControl>();

                var busHostedService = new BusHostedService(busControlMock.Object);
                await busHostedService.StartAsync(CancellationToken.None);

                busControlMock.Verify(control => control.StartAsync(It.IsAny<CancellationToken>()), Times.Once());
            }
        }

        /// <summary>
        ///     The stop async method.
        /// </summary>
        public class The_StopAsync_Method
        {
            /// <summary>
            ///     The calls the stop async method of bus control once.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Calls_The_StopAsync_Method_Of_BusControl_Once()
            {
                var busControlMock = new Mock<IBusControl>();

                var busHostedService = new BusHostedService(busControlMock.Object);
                await busHostedService.StopAsync(CancellationToken.None);

                busControlMock.Verify(control => control.StopAsync(It.IsAny<CancellationToken>()), Times.Once());
            }
        }
    }
}