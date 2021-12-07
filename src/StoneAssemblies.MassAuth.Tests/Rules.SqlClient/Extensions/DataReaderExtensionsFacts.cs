// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataReaderExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Rules.SqlClient.Extensions
{
    using System;
    using System.Data;
    using System.Linq;

    using Moq;

    using StoneAssemblies.MassAuth.Rules.SqlClient.Extensions;

    using Xunit;

    /// <summary>
    ///     The data reader extensions facts.
    /// </summary>
    public class DataReaderExtensionsFacts
    {
        /// <summary>
        ///     The the select method.
        /// </summary>
        public class The_GetAll_Method
        {
            /// <summary>
            ///     Returns a nom empty enumeration when read does not throw an exception.
            /// </summary>
            [Fact]
            public void Returns_A_Nom_Empty_Enumeration_When_Read_Does_Not_Throw_An_Exception()
            {
                var dataReaderMock = new Mock<IDataReader>();

                var count = 0;
                dataReaderMock.Setup(reader => reader.Read()).Callback(() => count++).Returns(() => count < 2);
                dataReaderMock.Setup(reader => reader.GetString(It.IsAny<int>())).Returns("string data");

                var collection = dataReaderMock.Object.GetAll(reader => reader.GetString(0)).ToList();

                Assert.NotEmpty(collection);
            }

            /// <summary>
            ///     Returns an empty enumeration when GetString throws an exception.
            /// </summary>
            [Fact]
            public void Returns_An_Empty_Enumeration_When_GetString_Throws_An_Exception()
            {
                var dataReaderMock = new Mock<IDataReader>();

                var count = 0;
                dataReaderMock.Setup(reader => reader.Read()).Callback(() => count++).Returns(() => count < 2);
                dataReaderMock.Setup(reader => reader.GetString(It.IsAny<int>())).Throws<Exception>();

                var collection = dataReaderMock.Object.GetAll(reader => reader.GetString(0)).ToList();

                Assert.Empty(collection);
            }

            /// <summary>
            ///     Returns an empty enumeration when read throws an exception.
            /// </summary>
            [Fact]
            public void Returns_An_Empty_Enumeration_When_Read_Throws_An_Exception()
            {
                var dataReaderMock = new Mock<IDataReader>();

                dataReaderMock.Setup(reader => reader.Read()).Throws<Exception>();

                Assert.Empty(dataReaderMock.Object.GetAll(reader => reader.GetString(0)));
            }
        }
    }
}