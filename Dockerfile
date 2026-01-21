# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["device-manager-api/device-manager-api.csproj", "device-manager-api/"]
RUN dotnet restore "device-manager-api/device-manager-api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/device-manager-api"
RUN dotnet build "device-manager-api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "device-manager-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "device-manager-api.dll"]
