#!/bin/bash

echo "🔗 Testing Supabase PostgreSQL Connection..."

# Check if connection string is provided
if [ -z "$DATABASE_URL" ]; then
    echo "❌ DATABASE_URL environment variable is not set"
    echo "Please set DATABASE_URL with your Supabase connection string"
    echo "Example: export DATABASE_URL='Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432'"
    exit 1
fi

# Test connection using psql
echo "📊 Testing connection with psql..."
psql "$DATABASE_URL" -c "SELECT version();" 2>/dev/null

if [ $? -eq 0 ]; then
    echo "✅ Connection successful!"
    echo "📋 Database information:"
    psql "$DATABASE_URL" -c "SELECT current_database(), current_user, version();"
else
    echo "❌ Connection failed!"
    echo "Please check your connection string and network connectivity"
fi

# Test connection using .NET
echo "🔧 Testing connection with .NET EF Core..."
cd src/HniDashOps.API

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET is not installed. Please install .NET 9 SDK first"
    exit 1
fi

# Test EF Core connection
dotnet ef database update --dry-run 2>/dev/null

if [ $? -eq 0 ]; then
    echo "✅ EF Core connection successful!"
else
    echo "❌ EF Core connection failed!"
    echo "Please check your connection string in appsettings.json"
fi

echo "🎉 Connection test completed!"
