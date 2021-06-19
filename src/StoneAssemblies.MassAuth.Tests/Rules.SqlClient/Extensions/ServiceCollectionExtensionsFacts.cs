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

    public class ServiceCollectionExtensionsFacts
    {
        public class The_RegisterStoredProcedureBasedRules_Method
        {
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