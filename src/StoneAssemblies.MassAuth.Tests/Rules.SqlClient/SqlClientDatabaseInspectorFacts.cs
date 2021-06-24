// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientDatabaseInspectorFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Rules.SqlClient
{
    using System.Data;
    using System.Linq;

    using Moq;

    using StoneAssemblies.MassAuth.Rules.SqlClient.Services;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces;

    using Xunit;

    /// <summary>
    ///     The sql client database inspector facts.
    /// </summary>
    public class SqlClientDatabaseInspectorFacts
    {
        /// <summary>
        ///     The GetMappings method.
        /// </summary>
        public class The_GetMappings_Method
        {
            /// <summary>
            ///     Does not throw exception with invalid connection strings.
            /// </summary>
            [Fact]
            public void Does_Not_Throw_Exception_With_Invalid_ConnectionStrings()
            {
                var connectionString =
                    "Server=localhost,321;Database=DB;User Id=User;Password=Password;MultipleActiveResultSets=true;Connection Timeout=30";
                var sqlClientDatabaseInspector = new SqlClientDatabaseInspector(new SqlClientConnectionFactory(), connectionString);
                var mappings = sqlClientDatabaseInspector.GetMappings().ToList();
                Assert.Empty(mappings);
            }

            /// <summary>
            /// The returns a non empty list of mappings.
            /// </summary>
            [Fact]
            public void Returns_A_Non_Empty_List_Of_Mappings()
            {
                var connectionString =
                    "Server=localhost,321;Database=DB;User Id=User;Password=Password;MultipleActiveResultSets=true;Connection Timeout=30";

                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.SetupGet(connection => connection.State).Returns(ConnectionState.Open);

                var storedProceduresCommandMock = new Mock<IDbCommand>();
                var storedProceduresDataReaderMock = new Mock<IDataReader>();
                var storedProceduresReadCount = 0;
                storedProceduresDataReaderMock.Setup(reader => reader.Read()).Returns(
                    () =>
                        {
                            storedProceduresReadCount++;
                            return storedProceduresReadCount < 2;
                        });

                storedProceduresDataReaderMock.Setup(reader => reader.GetString(2)).Returns("sp_Authorize_AccountBalanceRequest");
                storedProceduresCommandMock.Setup(command => command.ExecuteReader()).Returns(storedProceduresDataReaderMock.Object);

                var mappingsCommandMock = new Mock<IDbCommand>();
                var mappingsDataReaderMock = new Mock<IDataReader>();
                var mappingReadCount = 0;
                mappingsDataReaderMock.Setup(reader => reader.Read()).Returns(
                    () =>
                        {
                            mappingReadCount++;
                            return mappingReadCount < 2;
                        });

                mappingsDataReaderMock.Setup(reader => reader.GetString(0)).Returns("Is Account Owner");
                mappingsDataReaderMock.Setup(reader => reader.GetString(1)).Returns("AccountBalanceRequestMessage");
                mappingsDataReaderMock.Setup(reader => reader.GetString(2)).Returns("sp_Authorize_AccountBalanceRequest");
                mappingsDataReaderMock.Setup(reader => reader.GetInt32(3)).Returns(3);

                mappingsCommandMock.Setup(command => command.ExecuteReader()).Returns(mappingsDataReaderMock.Object);

                var times = 0;
                connectionMock.Setup(connection => connection.CreateCommand()).Returns(
                    () =>
                        {
                            if (times == 0)
                            {
                                times++;
                                return storedProceduresCommandMock.Object;
                            }

                            return mappingsCommandMock.Object;
                        });

                connectionFactoryMock.Setup(factory => factory.Create(connectionString)).Returns(connectionMock.Object);
                var sqlClientDatabaseInspector = new SqlClientDatabaseInspector(connectionFactoryMock.Object, connectionString);

                var mappings = sqlClientDatabaseInspector.GetMappings().ToList();

                Assert.NotEmpty(mappings);
            }
        }

        /// <summary>
        ///     The GetStoredProcedures method.
        /// </summary>
        public class The_GetStoredProcedures_Method
        {
            /// <summary>
            ///     Does not throw exception with invalid connection strings.
            /// </summary>
            [Fact]
            public void Does_Not_Throw_Exception_With_Invalid_ConnectionStrings()
            {
                var connectionString =
                    "Server=localhost,321;Database=DB;User Id=User;Password=Password;MultipleActiveResultSets=true;Connection Timeout=30";
                var sqlClientDatabaseInspector = new SqlClientDatabaseInspector(new SqlClientConnectionFactory(), connectionString);
                var storedProcedures = sqlClientDatabaseInspector.GetStoredProcedures().ToList();
                Assert.Empty(storedProcedures);
            }

            /// <summary>
            /// Returns a non empty list of_ stored procedures names.
            /// </summary>
            [Fact]
            public void Returns_A_Non_Empty_List_Of_StoredProcedures_Names()
            {
                var connectionString =
                    "Server=localhost,321;Database=DB;User Id=User;Password=Password;MultipleActiveResultSets=true;Connection Timeout=30";

                var connectionFactoryMock = new Mock<IConnectionFactory>();
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.SetupGet(connection => connection.State).Returns(ConnectionState.Open);

                var commandMock = new Mock<IDbCommand>();
                var dataReaderMock = new Mock<IDataReader>();
                var count = 0;
                dataReaderMock.Setup(reader => reader.Read()).Returns(
                    () =>
                        {
                            count++;
                            return count < 2;
                        });

                dataReaderMock.Setup(reader => reader.GetString(2)).Returns("sp_Authorize_AccountBalanceRequest");

                commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);
                connectionMock.Setup(connection => connection.CreateCommand()).Returns(commandMock.Object);
                connectionFactoryMock.Setup(factory => factory.Create(connectionString)).Returns(connectionMock.Object);
                var sqlClientDatabaseInspector = new SqlClientDatabaseInspector(connectionFactoryMock.Object, connectionString);
                var storedProcedures = sqlClientDatabaseInspector.GetStoredProcedures().ToList();

                Assert.NotEmpty(storedProcedures);
            }
        }
    }
}