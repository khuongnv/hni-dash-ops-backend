# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/HniDashOps.API/HniDashOps.API.csproj", "src/HniDashOps.API/"]
COPY ["src/HniDashOps.Core/HniDashOps.Core.csproj", "src/HniDashOps.Core/"]
COPY ["src/HniDashOps.Infrastructure/HniDashOps.Infrastructure.csproj", "src/HniDashOps.Infrastructure/"]
COPY ["src/HniDashOps.Shared/HniDashOps.Shared.csproj", "src/HniDashOps.Shared/"]

# Restore dependencies
RUN dotnet restore "src/HniDashOps.API/HniDashOps.API.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/HniDashOps.API"
RUN dotnet build "HniDashOps.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "HniDashOps.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Set environment variables for Railway
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:$PORT/health || exit 1

ENTRYPOINT ["dotnet", "HniDashOps.API.dll"]
