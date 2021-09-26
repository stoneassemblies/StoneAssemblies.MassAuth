string NuGetVersionV2 = "";
string SolutionFileName = "src/StoneAssemblies.MassAuth.sln";

string[] DockerFiles = new [] { 
	"./deployment/docker/StoneAssemblies.MassAuth.Server/Dockerfile",
	"./deployment/docker/StoneAssemblies.MassAuth.Proxy/Dockerfile",
	"./deployment/docker/StoneAssemblies.MassAuth.Bank.Balance.Services/Dockerfile" 
};

string[] OutputImages = new [] { 
	"stoneassemblies/massauth-server",
	"stoneassemblies/massauth-proxy",
	"stoneassemblies/massauth-bank-balance-services"
};

string[] ComponentProjects  = new [] {
	"./src/StoneAssemblies.MassAuth/StoneAssemblies.MassAuth.csproj",
	"./src/StoneAssemblies.MassAuth.Hosting/StoneAssemblies.MassAuth.Hosting.csproj",
	"./src/StoneAssemblies.MassAuth.Messages/StoneAssemblies.MassAuth.Messages.csproj",
	"./src/StoneAssemblies.MassAuth.Rules/StoneAssemblies.MassAuth.Rules.csproj",
	"./src/StoneAssemblies.MassAuth.Rules.SqlClient/StoneAssemblies.MassAuth.Rules.SqlClient.csproj",
	"./src/StoneAssemblies.MassAuth.Bank.Messages/StoneAssemblies.MassAuth.Bank.Messages.csproj",
	"./src/StoneAssemblies.MassAuth.Bank.Rules/StoneAssemblies.MassAuth.Bank.Rules.csproj"
	"./src/StoneAssemblies.Contrib.MassTransit/StoneAssemblies.Contrib.MassTransit.csproj"
};

string TestProject = "src/StoneAssemblies.MassAuth.Tests/StoneAssemblies.MassAuth.Tests.csproj";

// string SonarProjectKey = "stoneassemblies_StoneAssemblies.MassAuth";
// string SonarOrganization = "stoneassemblies";

string SonarProjectKey = "";
string SonarOrganization = "";