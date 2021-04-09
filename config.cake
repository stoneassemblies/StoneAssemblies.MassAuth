string NuGetVersionV2 = "";
string SolutionFileName = "src/StoneAssemblies.MassAuth.sln";

string[] DockerFiles = new [] { 
	"./deployment/docker/StoneAssemblies.MassAuth.Server/Dockerfile" 
};

string[] OutputImages = new [] { 
	"massauth-server" 
};

string[] ComponentProjects  = new [] {
	"./src/StoneAssemblies.MassAuth/StoneAssemblies.MassAuth.csproj",
	"./src/StoneAssemblies.MassAuth.Messages/StoneAssemblies.MassAuth.Messages.csproj",
	"./src/StoneAssemblies.MassAuth.Rules/StoneAssemblies.MassAuth.Rules.csproj",
};