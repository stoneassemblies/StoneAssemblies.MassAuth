// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Rules.SqlClient.Extensions
{
    using System;
    using System.Data;

    using Moq;

    using StoneAssemblies.MassAuth.Rules.SqlClient.Extensions;

    using Xunit;

    /// <summary>
    ///     The command extensions facts.
    /// </summary>
    public class CommandExtensionsFacts
    {
        /// <summary>
        ///     The execute reader safety method.
        /// </summary>
        public class The_ExecuteReaderSafety_Method
        {
            /// <summary>
            ///     Returns a not null data reader when execute reader returns a not null data reader.
            /// </summary>
            [Fact]
            public void Returns_A_Not_Null_DataReader_When_ExecuteReader_Returns_A_Not_Null_DataReader()
            {
                var databaseCommandMock = new Mock<IDbCommand>();
                databaseCommandMock.Setup(command => command.ExecuteReader()).Returns(Mock.Of<IDataReader>());

                var dataReader = databaseCommandMock.Object.ExecuteReaderSafety();

                Assert.NotNull(dataReader);
            }

            /// <summary>
            ///     Returns a null data reader when execute reader throws an exception.
            /// </summary>
            [Fact]
            public void Returns_A_Null_DataReader_When_ExecuteReader_Throws_An_Exception()
            {
                var databaseCommandMock = new Mock<IDbCommand>();
                databaseCommandMock.Setup(command => command.ExecuteReader()).Throws<Exception>();

                var dataReader = databaseCommandMock.Object.ExecuteReaderSafety();

                Assert.Null(dataReader);
            }
        }

   
    }
}