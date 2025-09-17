#!/bin/bash

# Docker build script for HniDashOps Backend
echo "🐳 Building HniDashOps Backend Docker image..."

# Build the Docker image
docker build -t hni-dash-ops-backend:latest .

if [ $? -eq 0 ]; then
    echo "✅ Docker image built successfully!"
    echo "📦 Image: hni-dash-ops-backend:latest"
    echo ""
    echo "🚀 To run the application:"
    echo "   docker-compose up"
    echo ""
    echo "🔍 To run in background:"
    echo "   docker-compose up -d"
    echo ""
    echo "🛑 To stop:"
    echo "   docker-compose down"
else
    echo "❌ Docker build failed!"
    exit 1
fi
