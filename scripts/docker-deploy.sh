#!/bin/bash

# Docker deployment script for Railway
echo "🚂 Preparing HniDashOps Backend for Railway deployment..."

# Check if Railway CLI is installed
if ! command -v railway &> /dev/null; then
    echo "❌ Railway CLI not found. Please install it first:"
    echo "   npm install -g @railway/cli"
    echo "   or visit: https://docs.railway.app/develop/cli"
    exit 1
fi

# Login to Railway (if not already logged in)
echo "🔐 Checking Railway authentication..."
railway whoami > /dev/null 2>&1
if [ $? -ne 0 ]; then
    echo "Please login to Railway:"
    railway login
fi

# Create new project (if not exists)
echo "📦 Creating Railway project..."
railway project create hni-dash-ops-backend

# Add PostgreSQL service
echo "🗄️ Adding PostgreSQL service..."
railway add postgresql

# Deploy the application
echo "🚀 Deploying to Railway..."
railway up

echo "✅ Deployment completed!"
echo ""
echo "🔗 Your application will be available at:"
echo "   https://hni-dash-ops-backend-production.up.railway.app"
echo ""
echo "📊 To view logs:"
echo "   railway logs"
echo ""
echo "🔧 To connect to database:"
echo "   railway connect postgresql"
