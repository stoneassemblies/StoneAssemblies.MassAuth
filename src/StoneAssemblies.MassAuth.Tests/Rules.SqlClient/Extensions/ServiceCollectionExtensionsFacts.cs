// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensionsFacts.cs" company="Stone Assemblies">
//  Copyright © 2021 - 2021 Stone Assemblies. All rights reserved. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Rules.SqlClient.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Extensions;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces;

    using Xunit;

    /// <summary>
    /// The service collection extensions facts.
    /// </summary>
    public class ServiceCollectionExtensionsFacts
    {
        /// <summary>
        /// The register stored procedure based rules method.
        /// </summary>
        public class The_RegisterStoredProcedureBasedRules_Method
        {
            /// <summary>
            /// Registers rules per stored procedure name matches with the specified_ pattern.
            /// </summary>
            [Fact]
            public void Registers_Rules_Per_Stored_Procedure_Name_Matches_With_The_Specified_Pattern()
            {
                var serviceCollection = new ServiceCollection();
                var patterns = new List<string>
                                   {
                                       "sp_Authorize_(?<ruleName>[^_]+)_(?<messageType>.+)",
                                       "sp_Authorize_(?<messageType>.+)",
                                   };

                var databaseInspectorMock = new Mock<IDatabaseInspector>();
                databaseInspectorMock.SetupGet(inspector => inspector.ConnectionString).Returns("Server=Fake");
                var storedProcedureNames = new List<string>();

                // Match
                storedProcedureNames.Add("sp_Authorize_IsAccountOwner_AccountBalanceRequestMessage");
                storedProcedureNames.Add("sp_Authorize_AccountBalanceRequestMessage");

                // Doesn't match
                storedProcedureNames.Add("sp_xAuthorize_IsAccountOwner_AccountBalanceRequestMessage");
                storedProcedureNames.Add("sp_xAuthorize_AccountBalanceRequestMessage");
                storedProcedureNames.Add("sp_Authorize_AccountBalanceRequestMessageRandomText");
                storedProcedureNames.Add("sp_Authorize_AccountBalanceRequestMessage_RandomText");
                storedProcedureNames.Add("Authorize_AccountBalanceRequestMessage");

                databaseInspectorMock.Setup(inspector => inspector.GetStoredProcedures()).Returns(
                    storedProcedureNames);

                serviceCollection.RegisterStoredProcedureBasedRules(databaseInspectorMock.Object, patterns);
                var provider = serviceCollection.BuildServiceProvider();
                var services = provider.GetServices<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>()
                    .ToList();

                Assert.Equal(2, services.Count);
            }

            /// <summary>
            /// Registers only one rule per stored procedure.
            /// </summary>
            [Fact]
            public void Registers_Only_One_Rule_Per_Stored_Procedure()
            {
                var serviceCollection = new ServiceCollection();
                var patterns = new List<string>
                                   {
                                       "sp_Authorize_(?<ruleName>[^_]+)_(?<messageType>.+)",
                                       "sp_Authorize_(?<messageType>.+)",
                                   };

                var databaseInspectorMock = new Mock<IDatabaseInspector>();
                databaseInspectorMock.SetupGet(inspector => inspector.ConnectionString).Returns("Server=Fake");
                var storedProcedureNames = new List<string>();

                // Match
                storedProcedureNames.Add("sp_Authorize_IsAccountOwner_AccountBalanceRequestMessage");
                storedProcedureNames.Add("sp_Authorize_IsAccountOwner_AccountBalanceRequestMessage");
                storedProcedureNames.Add("sp_Authorize_AccountBalanceRequestMessage");
                storedProcedureNames.Add("sp_Authorize_AccountBalanceRequestMessage");

                // Doesn't match
                storedProcedureNames.Add("sp_xAuthorize_IsAccountOwner_AccountBalanceRequestMessage");
                storedProcedureNames.Add("sp_xAuthorize_AccountBalanceRequestMessage");
                storedProcedureNames.Add("sp_Authorize_AccountBalanceRequestMessageRandomText");
                storedProcedureNames.Add("sp_Authorize_AccountBalanceRequestMessage_RandomText");
                storedProcedureNames.Add("Authorize_AccountBalanceRequestMessage");

                databaseInspectorMock.Setup(inspector => inspector.GetStoredProcedures()).Returns(
                    storedProcedureNames);

                serviceCollection.RegisterStoredProcedureBasedRules(databaseInspectorMock.Object, patterns);
                var provider = serviceCollection.BuildServiceProvider();
                var services = provider.GetServices<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>()
                    .ToList();

                Assert.Equal(2, services.Count);
            }
        }
    }
}