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

    using StoneAssemblies.Data.Services.Interfaces;
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Rules;

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
            public async Task Calls_The_Open_ExecuteReader_Close_And_Dispose_Methods_In_The_Correct_Order()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                var order = 0;
                connectionMock.Setup(connection => connection.Open()).Callback(() => Assert.Equal(0, order++));
                connectionMock.Setup(connection => connection.Close()).Callback(() => Assert.Equal(2, order++));
                connectionMock.Setup(connection => connection.Dispose()).Callback(() => Assert.Equal(3, order++));

                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);
                var commandMock = new Mock<IDbCommand>();

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(true);
                dataReaderMock.Setup(reader => reader.GetBoolean(0)).Returns(true);
                dataReaderMock.Setup(reader => reader.FieldCount).Returns(1);
                commandMock.Setup(command => command.ExecuteReader()).Callback(() => Assert.Equal(1, order++))
                    .Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                connectionMock.Verify(connection => connection.Open(), Times.Once);
                connectionMock.Verify(connection => connection.Close(), Times.Once);
                connectionMock.Verify(connection => connection.Dispose(), Times.Once);
                commandMock.Verify(connection => connection.ExecuteReader(), Times.Once);
            }

            [Fact]
            public async Task Returns_False_When_GetBoolean_From_Reader_Returns_False()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);
                var commandMock = new Mock<IDbCommand>();

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(true);
                dataReaderMock.Setup(reader => reader.GetBoolean(0)).Returns(false);
                dataReaderMock.Setup(reader => reader.FieldCount).Returns(1);

                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                Assert.False(result.Succeeded);
            }

            [Fact]
            public async Task Returns_False_When_Read_From_Reader_Returns_False()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);
                var commandMock = new Mock<IDbCommand>();

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(false);

                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                Assert.False(result.Succeeded);
            }

            [Fact]
            public async Task Returns_The_ForbiddanceReason_From_The_GetString_From_Reader()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);
                var commandMock = new Mock<IDbCommand>();

                const string ExpectedForbiddanceReason = "Forbiddance Reason";

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(true);
                dataReaderMock.Setup(reader => reader.FieldCount).Returns(2);
                dataReaderMock.Setup(reader => reader.GetFieldType(0)).Returns(typeof(bool));
                dataReaderMock.Setup(reader => reader.GetBoolean(0)).Returns(false);
                dataReaderMock.Setup(reader => reader.GetFieldType(1)).Returns(typeof(string));
                dataReaderMock.Setup(reader => reader.GetString(1)).Returns(ExpectedForbiddanceReason);

                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                Assert.Equal(ExpectedForbiddanceReason, result.Description);
            }

            /// <summary>
            ///     Returns true when execute scalar from command returns 1.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Returns_True_When_GetBoolean_From_Reader_Returns_True()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(true);
                dataReaderMock.Setup(reader => reader.GetFieldType(0)).Returns(typeof(bool));
                dataReaderMock.Setup(reader => reader.GetBoolean(0)).Returns(true);
                dataReaderMock.Setup(reader => reader.FieldCount).Returns(1);

                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                Assert.True(result.Succeeded);
            }

            /// <summary>
            /// Returns true when GetInt16 from reader returns 1.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            [Fact]
            public async Task Returns_True_When_GetInt16_From_Reader_Returns_1()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(true);
                dataReaderMock.Setup(reader => reader.GetFieldType(0)).Returns(typeof(short));
                dataReaderMock.Setup(reader => reader.GetInt16(0)).Returns(1);
                dataReaderMock.Setup(reader => reader.FieldCount).Returns(1);

                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                Assert.True(result.Succeeded);
            }

            /// <summary>
            /// Returns true when GetInt32 from reader returns 1.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            [Fact]
            public async Task Returns_True_When_GetInt32_From_Reader_Returns_1()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(true);
                dataReaderMock.Setup(reader => reader.GetFieldType(0)).Returns(typeof(int));
                dataReaderMock.Setup(reader => reader.GetInt32(0)).Returns(1);
                dataReaderMock.Setup(reader => reader.FieldCount).Returns(1);

                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                Assert.True(result.Succeeded);
            }

            /// <summary>
            /// Returns true when GetInt64 from reader returns 1.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            [Fact]
            public async Task Returns_True_When_GetInt64_From_Reader_Returns_1()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(true);
                dataReaderMock.Setup(reader => reader.GetFieldType(0)).Returns(typeof(long));
                dataReaderMock.Setup(reader => reader.GetInt64(0)).Returns(1);
                dataReaderMock.Setup(reader => reader.FieldCount).Returns(1);

                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                Assert.True(result.Succeeded);
            }

            /// <summary>
            /// Returns false when GetInt16 from reader returns 0.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            [Fact]
            public async Task Returns_False_When_GetInt16_From_Reader_Returns_0()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(true);
                dataReaderMock.Setup(reader => reader.GetFieldType(0)).Returns(typeof(short));
                dataReaderMock.Setup(reader => reader.GetInt16(0)).Returns(0);
                dataReaderMock.Setup(reader => reader.FieldCount).Returns(1);

                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                Assert.False(result.Succeeded);
            }

            /// <summary>
            /// Returns false when GetInt32 from reader returns 0.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            [Fact]
            public async Task Returns_False_When_GetInt32_From_Reader_Returns_0()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(true);
                dataReaderMock.Setup(reader => reader.GetFieldType(0)).Returns(typeof(int));
                dataReaderMock.Setup(reader => reader.GetInt32(0)).Returns(0);
                dataReaderMock.Setup(reader => reader.FieldCount).Returns(1);

                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                Assert.False(result.Succeeded);
            }

            /// <summary>
            /// Returns false when GetInt64 from reader returns 0.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            [Fact]
            public async Task Returns_False_When_GetInt64_From_Reader_Returns_0()
            {
                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(connection => connection.State).Returns(ConnectionState.Open);

                var dataReaderMock = new Mock<IDataReader>();
                dataReaderMock.Setup(reader => reader.Read()).Returns(true);
                dataReaderMock.Setup(reader => reader.GetFieldType(0)).Returns(typeof(long));
                dataReaderMock.Setup(reader => reader.GetInt64(0)).Returns(0);
                dataReaderMock.Setup(reader => reader.FieldCount).Returns(1);

                var commandMock = new Mock<IDbCommand>();
                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

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
                                         PrimaryAccountNumber = "1341234",
                                     },
                                 });

                Assert.False(result.Succeeded);
            }
        }
    }

    /// <summary>
    ///     The name property.
    /// </summary>
    public class The_Name_Property
    {
        /// <summary>
        ///     returns the name of the rule.
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
        ///     Returns the stored procedure name as part of the name of the rule.
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
    ///     The priory property.
    /// </summary>
    public class The_Priory_Property
    {
        /// <summary>
        ///     Returns specified value.
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