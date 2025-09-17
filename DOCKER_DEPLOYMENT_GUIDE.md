# 🐳 Docker Deployment Guide cho HniDashOps Backend

## 📋 Tổng Quan

Project đã được containerized với Docker và sẵn sàng deploy lên Railway hoặc bất kỳ platform nào hỗ trợ Docker.

## 🗂️ Files Đã Tạo

### ✅ **Docker Files**
- `Dockerfile` - Multi-stage build cho .NET 9 API
- `.dockerignore` - Loại bỏ files không cần thiết
- `docker-compose.yml` - Local development với PostgreSQL
- `docker-compose.override.yml` - Development overrides
- `railway.json` - Railway deployment configuration

### ✅ **Scripts**
- `scripts/docker-build.sh` - Build Docker image
- `scripts/docker-deploy.sh` - Deploy lên Railway

---

## 🚀 Local Development với Docker

### 1. **Build và Run với Docker Compose**

```bash
# Build và start tất cả services
docker-compose up --build

# Run trong background
docker-compose up -d --build

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

### 2. **Access Application**

- **API**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger
- **Health Check**: http://localhost:8080/health
- **PostgreSQL**: localhost:5432

### 3. **Database Setup**

```bash
# Apply migrations
docker-compose exec api dotnet ef database update --project /src/HniDashOps.Infrastructure --startup-project /src/HniDashOps.API

# Seed initial data
curl -X POST http://localhost:8080/api/seed/seed
```

---

## 🚂 Deploy lên Railway

### **Phương Pháp 1: Railway CLI (Recommended)**

#### 1. **Install Railway CLI**

```bash
# Install via npm
npm install -g @railway/cli

# Or via curl (Linux/macOS)
curl -fsSL https://railway.app/install.sh | sh
```

#### 2. **Login và Deploy**

```bash
# Login to Railway
railway login

# Create new project
railway project create hni-dash-ops-backend

# Add PostgreSQL service
railway add postgresql

# Deploy
railway up
```

#### 3. **Configure Environment Variables**

Trong Railway Dashboard, thêm các environment variables:

```bash
# Database (Railway sẽ tự động tạo)
DATABASE_URL=postgresql://username:password@host:port/database

# JWT Settings
JWT_SECRET=your-super-secret-jwt-key-for-production
JWT_ISSUER=HniDashOps
JWT_AUDIENCE=HniDashOps-Users
JWT_EXPIRY_MINUTES=60

# CORS
CORS_ORIGINS=https://your-nuxt-app.vercel.app

# Application
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
```

### **Phương Pháp 2: Railway Dashboard**

#### 1. **Connect GitHub Repository**

1. Truy cập [Railway.app](https://railway.app)
2. Click "New Project"
3. Select "Deploy from GitHub repo"
4. Chọn repository `hni-dash-ops-backend`

#### 2. **Add PostgreSQL Service**

1. Click "New Service"
2. Select "Database" → "PostgreSQL"
3. Railway sẽ tự động tạo connection string

#### 3. **Configure Environment Variables**

Thêm các environment variables như trên.

#### 4. **Deploy**

Railway sẽ tự động detect `Dockerfile` và deploy.

---

## 🔧 Docker Commands

### **Build Commands**

```bash
# Build Docker image
docker build -t hni-dash-ops-backend:latest .

# Build với specific tag
docker build -t hni-dash-ops-backend:v1.0.0 .

# Build và run
docker run -p 8080:8080 hni-dash-ops-backend:latest
```

### **Docker Compose Commands**

```bash
# Start services
docker-compose up

# Start in background
docker-compose up -d

# Rebuild và start
docker-compose up --build

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Stop và remove volumes
docker-compose down -v

# Execute commands in container
docker-compose exec api dotnet --version
docker-compose exec postgres psql -U postgres -d hni_dash_ops
```

### **Debugging Commands**

```bash
# Check container status
docker-compose ps

# View container logs
docker-compose logs api
docker-compose logs postgres

# Access container shell
docker-compose exec api /bin/bash
docker-compose exec postgres /bin/bash

# Check database connection
docker-compose exec api dotnet ef database update --project /src/HniDashOps.Infrastructure --startup-project /src/HniDashOps.API
```

---

## 🏗️ Dockerfile Architecture

### **Multi-Stage Build**

```dockerfile
# Stage 1: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Restore dependencies
# Copy source code
# Build application

# Stage 3: Publish
FROM build AS publish
# Publish application

# Stage 4: Final runtime
FROM base AS final
# Copy published files
# Set up non-root user
# Configure environment
```

### **Security Features**

- ✅ **Non-root user** cho security
- ✅ **Health checks** cho monitoring
- ✅ **Multi-stage build** để giảm image size
- ✅ **Environment variables** cho configuration

---

## 📊 Performance Optimization

### **Image Size Optimization**

- ✅ **Multi-stage build** - Chỉ copy runtime files
- ✅ **.dockerignore** - Loại bỏ unnecessary files
- ✅ **Alpine base images** - Smaller footprint
- ✅ **Layer caching** - Faster rebuilds

### **Runtime Optimization**

- ✅ **Health checks** - Container health monitoring
- ✅ **Resource limits** - Prevent resource exhaustion
- ✅ **Environment variables** - Flexible configuration
- ✅ **Port exposure** - Proper networking

---

## 🔍 Troubleshooting

### **Common Issues**

#### 1. **Build Fails**

```bash
# Check Docker is running
docker --version

# Clean build cache
docker system prune -a

# Rebuild without cache
docker build --no-cache -t hni-dash-ops-backend:latest .
```

#### 2. **Database Connection Issues**

```bash
# Check PostgreSQL is running
docker-compose ps postgres

# Check connection string
docker-compose exec api env | grep ConnectionStrings

# Test database connection
docker-compose exec postgres pg_isready -U postgres
```

#### 3. **Port Conflicts**

```bash
# Check port usage
netstat -tulpn | grep :8080
netstat -tulpn | grep :5432

# Change ports in docker-compose.yml
ports:
  - "8081:8080"  # Use different host port
```

#### 4. **Railway Deployment Issues**

```bash
# Check Railway status
railway status

# View deployment logs
railway logs

# Check environment variables
railway variables

# Redeploy
railway up --detach
```

---

## 🎯 Production Checklist

### ✅ **Before Deployment**

- [ ] Environment variables configured
- [ ] Database migrations applied
- [ ] Health checks working
- [ ] CORS origins set correctly
- [ ] JWT secret key is secure
- [ ] SSL/HTTPS enabled

### ✅ **After Deployment**

- [ ] API endpoints accessible
- [ ] Swagger documentation working
- [ ] Database connection successful
- [ ] Authentication working
- [ ] Health check endpoint responding
- [ ] Logs are being generated

---

## 📈 Monitoring & Logs

### **Railway Monitoring**

```bash
# View real-time logs
railway logs --follow

# View specific service logs
railway logs --service api

# Check deployment status
railway status
```

### **Health Checks**

- **Endpoint**: `/health`
- **Interval**: 30 seconds
- **Timeout**: 3 seconds
- **Retries**: 3

---

## 🚀 Next Steps

1. **Deploy to Railway** using the provided scripts
2. **Configure environment variables** in Railway dashboard
3. **Apply database migrations** after deployment
4. **Seed initial data** using the seed endpoint
5. **Test all API endpoints** to ensure functionality
6. **Set up monitoring** and alerting
7. **Configure CI/CD** for automated deployments

---

**🎉 Docker deployment setup hoàn thành! Project sẵn sàng deploy lên Railway hoặc bất kỳ platform nào hỗ trợ Docker.**
