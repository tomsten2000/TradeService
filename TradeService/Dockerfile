# Stadie 1
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stadie 2
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TradeService/TradeService.csproj", "TradeService/"]
RUN dotnet restore "./TradeService/TradeService.csproj"
COPY . .
WORKDIR "/src/TradeService"
RUN dotnet build "./TradeService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stadie 3
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TradeService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stadie 4
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TradeService.dll"]