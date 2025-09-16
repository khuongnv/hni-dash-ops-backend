#!/bin/bash

echo "🔗 Supabase Connection String Generator"
echo "======================================"

# Get input from user
read -p "Enter Supabase Host (db.xxx.supabase.co): " HOST
read -p "Enter Database Password: " -s PASSWORD
echo ""

# Validate input
if [ -z "$HOST" ] || [ -z "$PASSWORD" ]; then
    echo "❌ Error: Host and Password are required"
    exit 1
fi

# Generate connection string
CONNECTION_STRING="Host=${HOST};Database=postgres;Username=postgres;Password=${PASSWORD};Port=5432;SSL Mode=Require;Trust Server Certificate=true"

echo ""
echo "✅ Generated Connection String:"
echo "================================"
echo "$CONNECTION_STRING"
echo ""

# Save to file
echo "$CONNECTION_STRING" > connection-string.txt
echo "💾 Saved to connection-string.txt"

# Test connection
echo ""
echo "🧪 Testing connection..."
psql "$CONNECTION_STRING" -c "SELECT 'Connection successful!' as status;" 2>/dev/null

if [ $? -eq 0 ]; then
    echo "✅ Connection test successful!"
else
    echo "❌ Connection test failed!"
    echo "Please check your credentials and network connectivity"
fi
