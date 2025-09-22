#!/bin/bash

# Docker deployment script for HniDashOps Backend
echo "🚀 Preparing HniDashOps Backend for deployment..."

# Build Docker image
echo "🔨 Building Docker image..."
docker build -t hni-dash-ops-backend .

# Run the application with Docker Compose
echo "🐳 Starting application with Docker Compose..."
docker-compose up -d

echo "✅ Deployment completed!"
echo ""
echo "🔗 Your application is available at:"
echo "   http://localhost:8080"
echo ""
echo "📊 To view logs:"
echo "   docker-compose logs -f"
echo ""
echo "🔧 To stop the application:"
echo "   docker-compose down"
