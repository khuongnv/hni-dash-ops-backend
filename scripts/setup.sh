#!/bin/bash

echo "🚀 Setting up HniDashOps Backend..."

# Check .NET 9 installation
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 9 not found. Please install .NET 9 SDK"
    echo "📥 Download from: https://dotnet.microsoft.com/download/dotnet/9.0"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version)
echo "✅ .NET version: $DOTNET_VERSION"

# Create solution
echo "📁 Creating solution..."
dotnet new sln -n HniDashOps

# Create projects
echo "🏗️ Creating projects..."
dotnet new webapi -n HniDashOps.API -o src/HniDashOps.API
dotnet new classlib -n HniDashOps.Core -o src/HniDashOps.Core
dotnet new classlib -n HniDashOps.Infrastructure -o src/HniDashOps.Infrastructure
dotnet new classlib -n HniDashOps.Shared -o src/HniDashOps.Shared

# Add projects to solution
echo "🔗 Adding projects to solution..."
dotnet sln add src/HniDashOps.API/HniDashOps.API.csproj
dotnet sln add src/HniDashOps.Core/HniDashOps.Core.csproj
dotnet sln add src/HniDashOps.Infrastructure/HniDashOps.Infrastructure.csproj
dotnet sln add src/HniDashOps.Shared/HniDashOps.Shared.csproj

# Add project references
echo "🔗 Adding project references..."
dotnet add src/HniDashOps.API/HniDashOps.API.csproj reference src/HniDashOps.Core/HniDashOps.Core.csproj
dotnet add src/HniDashOps.API/HniDashOps.API.csproj reference src/HniDashOps.Infrastructure/HniDashOps.Infrastructure.csproj
dotnet add src/HniDashOps.API/HniDashOps.API.csproj reference src/HniDashOps.Shared/HniDashOps.Shared.csproj
dotnet add src/HniDashOps.Infrastructure/HniDashOps.Infrastructure.csproj reference src/HniDashOps.Core/HniDashOps.Core.csproj
dotnet add src/HniDashOps.Infrastructure/HniDashOps.Infrastructure.csproj reference src/HniDashOps.Shared/HniDashOps.Shared.csproj
dotnet add src/HniDashOps.Core/HniDashOps.Core.csproj reference src/HniDashOps.Shared/HniDashOps.Shared.csproj

# Create test projects
echo "🧪 Creating test projects..."
dotnet new xunit -n HniDashOps.API.Tests -o tests/HniDashOps.API.Tests
dotnet new xunit -n HniDashOps.Core.Tests -o tests/HniDashOps.Core.Tests

# Add test projects to solution
dotnet sln add tests/HniDashOps.API.Tests/HniDashOps.API.Tests.csproj
dotnet sln add tests/HniDashOps.Core.Tests/HniDashOps.Core.Tests.csproj

# Add test references
dotnet add tests/HniDashOps.API.Tests/HniDashOps.API.Tests.csproj reference src/HniDashOps.API/HniDashOps.API.csproj
dotnet add tests/HniDashOps.Core.Tests/HniDashOps.Core.Tests.csproj reference src/HniDashOps.Core/HniDashOps.Core.csproj

echo "✅ Setup completed!"
echo "📁 Project structure created"
echo "🚀 Ready to start development!"
echo ""
echo "Next steps:"
echo "1. Run: dotnet restore"
echo "2. Run: dotnet build"
echo "3. Run: dotnet run --project src/HniDashOps.API"
