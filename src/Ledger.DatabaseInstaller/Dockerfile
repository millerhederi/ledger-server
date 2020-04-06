FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/Ledger.DatabaseInstaller/Ledger.DatabaseInstaller.csproj src/Ledger.DatabaseInstaller/
RUN dotnet restore src/Ledger.DatabaseInstaller/Ledger.DatabaseInstaller.csproj

# copy everything else and build app
COPY src/Ledger.DatabaseInstaller/ src/Ledger.DatabaseInstaller/
WORKDIR /source/src/Ledger.DatabaseInstaller
RUN dotnet build -c release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish -c release --no-build -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ledger.DatabaseInstaller.dll"]