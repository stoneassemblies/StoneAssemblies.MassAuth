// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Rules.SqlClient.Extensions
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

    using Microsoft.Data.SqlClient;

    using Moq;

    using StoneAssemblies.MassAuth.Rules.SqlClient.Extensions;

    using Xunit;

    /// <summary>
    ///     The connection extensions facts.
    /// </summary>
    public class ConnectionExtensionsFacts
    {
        /// <summary>
        ///     The dispose async method.
        /// </summary>
        public class The_CloseAsync_Method
        {
            /// <summary>
            ///     Succeeds.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Succeeds()
            {
                var connection = new SqlConnection() as IDbConnection;

                await connection.CloseAsync();

                Assert.True(true);
            }
        }

        /// <summary>
        ///     The DisposeAsync method.
        /// </summary>
        public class The_DisposeAsync_Method
        {
            /// <summary>
            ///     Succeeds.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Calls_DisposeAsync_From_AsyncDisposable_Once()
            {
                var connectionMock = new Mock<IDbConnection>();
                var asyncDisposableMock = connectionMock.As<IAsyncDisposable>();

                await connectionMock.Object.DisposeAsync();

                asyncDisposableMock.Verify(disposable => disposable.DisposeAsync(), Times.Once);
            }
        }

        /// <summary>
        ///     The open async method.
        /// </summary>
        public class The_OpenAsync_Method
        {
            /// <summary>
            ///     Throws invalid operation exception because the connection string property has not been initialized.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Throws_InvalidOperationException_Because_The_ConnectionString_Property_Has_Not_Been_Initialized()
            {
                var connection = new SqlConnection() as IDbConnection;

                await Assert.ThrowsAsync<InvalidOperationException>(() => connection.OpenAsync());
            }
        }
    }
}