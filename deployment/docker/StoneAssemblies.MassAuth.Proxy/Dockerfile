ARG PACKAGE_VERSION=${PACKAGE_VERSION:-1.0.0}
ARG DOTNET_SDK_VERSION=${DOTNET_SDK_VERSION:-6.0}
ARG DOTNET_ASP_NETCORE_RUNTIME_VERSION=${DOTNET_ASP_NETCORE_RUNTIME_VERSION:-6.0}
ARG DOCKER_REPOSITORY_PROXY=${DOCKER_REPOSITORY_PROXY:-mcr.microsoft.com}

FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_SDK_VERSION AS build

ARG NUGET_REPOSITORY_PROXY=${NUGET_REPOSITORY_PROXY:-https://www.nuget.org/api/v2/}

WORKDIR /build/src
ADD dotnet.csproj.tar.gz .
RUN dotnet restore --source $NUGET_REPOSITORY_PROXY
COPY src/. .

WORKDIR /build/src/StoneAssemblies.MassAuth.Proxy
RUN dotnet publish -c Release -o ../../output/Release/StoneAssemblies.MassAuth.Proxy /p:ServerGarbageCollection=false

WORKDIR /build

FROM mcr.microsoft.com/dotnet/aspnet:$DOTNET_ASP_NETCORE_RUNTIME_VERSION
ENV ASPNETCORE_URLS=http://0.0.0.0:80
WORKDIR /app

COPY --from=build /build/output/Release/StoneAssemblies.MassAuth.Proxy .

RUN rm appsettings.json
RUN rm appsettings.Development.json

ENTRYPOINT ["dotnet", "StoneAssemblies.MassAuth.Proxy.dll"]