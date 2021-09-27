// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientStoredProcedureBasedRuleFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Rules.SqlClient.Rules
{
    using System.Data;
    using System.Threading.Tasks;

    using Moq;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Rules;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces;

    using Xunit;

    /// <summary>
    ///     The sql client stored procedure based rule facts.
    /// </summary>
    public class SqlClientStoredProcedureBasedRuleFacts
    {
        /// <summary>
        ///     The execute async method.
        /// </summary>
        public class The_ExecuteAsync_Method
        {
            /// <summary>
            ///     Calls the open execute scalar close and dispose methods in the correct order.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Calls_The_Open_ExecuteScalar_Close_And_Dispose_Methods_In_The_Correct_Order()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                var order = 0;
                connectionMock.Setup(connection => connection.Open()).Callback(() => Assert.Equal(0, order++));
                connectionMock.Setup(connection => connection.Close()).Callback(() => Assert.Equal(2, order++));
                connectionMock.Setup(connection => connection.Dispose()).Callback(() => Assert.Equal(3, order++));

                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);
                var commandMock = new Mock<IDbCommand>();

                commandMock.Setup(command => command.ExecuteScalar()).Callback(() => Assert.Equal(1, order++)).Returns(true);

                var parameterMock = new Mock<IDbDataParameter>();
                commandMock.Setup(command => command.CreateParameter()).Returns(parameterMock.Object);
                var parameterCollectionMock = new Mock<IDataParameterCollection>();
                commandMock.Setup(command => command.Parameters).Returns(parameterCollectionMock.Object);

                connectionMock.Setup(connection => connection.CreateCommand()).Returns(commandMock.Object);

                connectionFactoryMock.Setup(factory => factory.Create(It.IsAny<string>())).Returns(connectionMock.Object);

                var sqlClientStoredProcedureBasedRule =
                    new SqlClientStoredProcedureBasedRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        connectionFactoryMock.Object,
                        "TestRule",
                        typeof(AccountBalanceRequestMessage),
                        "Server=localhost;",
                        "sp_Authorize_AccountBalanceRequestMessage",
                        0);

                var result = await sqlClientStoredProcedureBasedRule.EvaluateAsync(
                                 new AuthorizationRequestMessage<AccountBalanceRequestMessage>
                                     {
                                         Payload = new AccountBalanceRequestMessage
                                                       {
                                                           PrimaryAccountNumber = "1341234"
                                                       }
                                     });

                connectionMock.Verify(connection => connection.Open(), Times.Once);
                connectionMock.Verify(connection => connection.Close(), Times.Once);
                connectionMock.Verify(connection => connection.Dispose(), Times.Once);
                commandMock.Verify(connection => connection.ExecuteScalar(), Times.Once);
            }

            /// <summary>
            ///     Returns false when execute scalar from command returns 0.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Returns_False_When_ExecuteScalar_From_Command_Returns_0()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);
                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteScalar()).Returns(0);

                var parameterMock = new Mock<IDbDataParameter>();
                commandMock.Setup(command => command.CreateParameter()).Returns(parameterMock.Object);
                var parameterCollectionMock = new Mock<IDataParameterCollection>();
                commandMock.Setup(command => command.Parameters).Returns(parameterCollectionMock.Object);

                connectionMock.Setup(connection => connection.CreateCommand()).Returns(commandMock.Object);

                connectionFactoryMock.Setup(factory => factory.Create(It.IsAny<string>())).Returns(connectionMock.Object);

                var sqlClientStoredProcedureBasedRule =
                    new SqlClientStoredProcedureBasedRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        connectionFactoryMock.Object,
                        "TestRule",
                        typeof(AccountBalanceRequestMessage),
                        "Server=localhost;",
                        "sp_Authorize_AccountBalanceRequestMessage",
                        0);

                var result = await sqlClientStoredProcedureBasedRule.EvaluateAsync(
                                 new AuthorizationRequestMessage<AccountBalanceRequestMessage>
                                     {
                                         Payload = new AccountBalanceRequestMessage
                                                       {
                                                           PrimaryAccountNumber = "1341234"
                                                       }
                                     });

                Assert.False(result);
            }

            /// <summary>
            ///     Returns false when execute scalar from command returns false.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Returns_False_When_ExecuteScalar_From_Command_Returns_False()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);
                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteScalar()).Returns(false);

                var parameterMock = new Mock<IDbDataParameter>();
                commandMock.Setup(command => command.CreateParameter()).Returns(parameterMock.Object);
                var parameterCollectionMock = new Mock<IDataParameterCollection>();
                commandMock.Setup(command => command.Parameters).Returns(parameterCollectionMock.Object);

                connectionMock.Setup(connection => connection.CreateCommand()).Returns(commandMock.Object);

                connectionFactoryMock.Setup(factory => factory.Create(It.IsAny<string>())).Returns(connectionMock.Object);

                var sqlClientStoredProcedureBasedRule =
                    new SqlClientStoredProcedureBasedRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        connectionFactoryMock.Object,
                        "TestRule",
                        typeof(AccountBalanceRequestMessage),
                        "Server=localhost;",
                        "sp_Authorize_AccountBalanceRequestMessage",
                        0);

                var result = await sqlClientStoredProcedureBasedRule.EvaluateAsync(
                                 new AuthorizationRequestMessage<AccountBalanceRequestMessage>
                                     {
                                         Payload = new AccountBalanceRequestMessage
                                                       {
                                                           PrimaryAccountNumber = "1341234"
                                                       }
                                     });

                Assert.False(result);
            }

            /// <summary>
            ///     Returns true when execute scalar from command returns 1.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Returns_True_When_ExecuteScalar_From_Command_Returns_1()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);
                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteScalar()).Returns(true);

                var parameterMock = new Mock<IDbDataParameter>();
                commandMock.Setup(command => command.CreateParameter()).Returns(parameterMock.Object);
                var parameterCollectionMock = new Mock<IDataParameterCollection>();
                commandMock.Setup(command => command.Parameters).Returns(parameterCollectionMock.Object);

                connectionMock.Setup(connection => connection.CreateCommand()).Returns(commandMock.Object);

                connectionFactoryMock.Setup(factory => factory.Create(It.IsAny<string>())).Returns(connectionMock.Object);

                var sqlClientStoredProcedureBasedRule =
                    new SqlClientStoredProcedureBasedRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        connectionFactoryMock.Object,
                        "TestRule",
                        typeof(AccountBalanceRequestMessage),
                        "Server=localhost;",
                        "sp_Authorize_AccountBalanceRequestMessage",
                        0);

                var result = await sqlClientStoredProcedureBasedRule.EvaluateAsync(
                                 new AuthorizationRequestMessage<AccountBalanceRequestMessage>
                                     {
                                         Payload = new AccountBalanceRequestMessage
                                                       {
                                                           PrimaryAccountNumber = "1341234"
                                                       }
                                     });

                Assert.True(result);
            }

            /// <summary>
            ///     Returns true when execute scalar from command returns true.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Returns_True_When_ExecuteScalar_From_Command_Returns_True()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);
                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteScalar()).Returns(true);

                var parameterMock = new Mock<IDbDataParameter>();
                commandMock.Setup(command => command.CreateParameter()).Returns(parameterMock.Object);
                var parameterCollectionMock = new Mock<IDataParameterCollection>();
                commandMock.Setup(command => command.Parameters).Returns(parameterCollectionMock.Object);

                connectionMock.Setup(connection => connection.CreateCommand()).Returns(commandMock.Object);

                connectionFactoryMock.Setup(factory => factory.Create(It.IsAny<string>())).Returns(connectionMock.Object);

                var sqlClientStoredProcedureBasedRule =
                    new SqlClientStoredProcedureBasedRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        connectionFactoryMock.Object,
                        "TestRule",
                        typeof(AccountBalanceRequestMessage),
                        "Server=localhost;",
                        "sp_Authorize_AccountBalanceRequestMessage",
                        0);

                var result = await sqlClientStoredProcedureBasedRule.EvaluateAsync(
                                 new AuthorizationRequestMessage<AccountBalanceRequestMessage>
                                     {
                                         Payload = new AccountBalanceRequestMessage
                                                       {
                                                           PrimaryAccountNumber = "1341234"
                                                       }
                                     });

                Assert.True(result);
            }
        }

        /// <summary>
        /// The name property.
        /// </summary>
        public class The_Name_Property
        {
            /// <summary>
            /// returns the name of the rule.
            /// </summary>
            [Fact]
            public void Returns_The_Name_Of_The_Rule()
            {
                var sqlClientStoredProcedureBasedRule =
                    new SqlClientStoredProcedureBasedRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        Mock.Of<IConnectionFactory>(),
                        "TestRule",
                        typeof(AccountBalanceRequestMessage),
                        "Server=localhost;",
                        "sp_Authorize_AccountBalanceRequestMessage",
                        0);

                Assert.Contains("TestRule", sqlClientStoredProcedureBasedRule.Name);
            }

            /// <summary>
            /// Returns the stored procedure name as part of the name of the rule.
            /// </summary>
            [Fact]
            public void Returns_The_StoredProcedureName_As_Part_Of_The_Name_Of_The_Rule()
            {
                var sqlClientStoredProcedureBasedRule =
                    new SqlClientStoredProcedureBasedRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        Mock.Of<IConnectionFactory>(),
                        string.Empty,
                        typeof(AccountBalanceRequestMessage),
                        "Server=localhost;",
                        "sp_Authorize_AccountBalanceRequestMessage",
                        0);

                Assert.Contains("sp_Authorize_AccountBalanceRequestMessage", sqlClientStoredProcedureBasedRule.Name);
            }
        }

        /// <summary>
        /// The priory property.
        /// </summary>
        public class The_Priory_Property
        {
            /// <summary>
            /// Returns specified value.
            /// </summary>
            [Fact]
            public void Returns_Specified_Value()
            {
                var sqlClientStoredProcedureBasedRule =
                    new SqlClientStoredProcedureBasedRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        Mock.Of<IConnectionFactory>(),
                        "TestRule",
                        typeof(AccountBalanceRequestMessage),
                        "Server=localhost;",
                        "sp_Authorize_AccountBalanceRequestMessage",
                        10);

                Assert.Equal(10, sqlClientStoredProcedureBasedRule.Priority);
            }
        }
    }
}