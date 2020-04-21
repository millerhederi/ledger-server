FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/Ledger.WebApi/Ledger.WebApi.csproj src/Ledger.WebApi/
RUN dotnet restore src/Ledger.WebApi/Ledger.WebApi.csproj

# copy everything else and build app
COPY src/Ledger.WebApi/ src/Ledger.WebApi/
WORKDIR /source/src/Ledger.WebApi
RUN dotnet build -c release --no-restore

FROM build AS publish
RUN dotnet publish -c release --no-build -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ledger.WebApi.dll"]