# 🚀 .NET 9 SDK Installation Guide

## 📋 Prerequisites Check

Trước khi cài đặt .NET 9 SDK, hãy kiểm tra hệ thống hiện tại:

```bash
# Kiểm tra OS version
uname -a

# Kiểm tra architecture
arch

# Kiểm tra available space
df -h
```

## 🍎 macOS Installation

### Method 1: Homebrew (Recommended)

```bash
# Cài đặt Homebrew (nếu chưa có)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Cài đặt .NET 9 SDK
brew install dotnet

# Verify installation
dotnet --version
```

### Method 2: Direct Download

1. **Download**: [.NET 9 SDK for macOS](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Choose**: macOS x64 hoặc macOS ARM64 (tùy theo Mac của bạn)
3. **Install**: Chạy file `.pkg` đã download
4. **Verify**: `dotnet --version` trong Terminal

### Method 3: Manual Installation

```bash
# Download .NET 9 SDK
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0

# Add to PATH (add to ~/.zshrc or ~/.bash_profile)
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.zshrc
echo 'export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools' >> ~/.zshrc

# Reload shell
source ~/.zshrc

# Verify installation
dotnet --version
```

## 🪟 Windows Installation

### Method 1: winget (Recommended)

```powershell
# Cài đặt .NET 9 SDK
winget install Microsoft.DotNet.SDK.9

# Verify installation
dotnet --version
```

### Method 2: Direct Download

1. **Download**: [.NET 9 SDK for Windows](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Choose**: Windows x64 hoặc Windows ARM64
3. **Install**: Chạy file `.exe` đã download
4. **Verify**: `dotnet --version` trong Command Prompt hoặc PowerShell

### Method 3: Visual Studio Installer

1. **Download**: [Visual Studio Installer](https://visualstudio.microsoft.com/downloads/)
2. **Install**: Visual Studio Community (free)
3. **Select**: .NET 9 development workload
4. **Install**: Follow installation wizard

## 🐧 Linux Installation

### Ubuntu/Debian

```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# Update package list
sudo apt-get update

# Install .NET 9 SDK
sudo apt-get install -y dotnet-sdk-9.0

# Verify installation
dotnet --version
```

### CentOS/RHEL/Fedora

```bash
# Add Microsoft package repository
sudo rpm -Uvh https://packages.microsoft.com/config/centos/8/packages-microsoft-prod.rpm

# Install .NET 9 SDK
sudo dnf install -y dotnet-sdk-9.0

# Verify installation
dotnet --version
```

### Manual Installation (All Linux)

```bash
# Download .NET 9 SDK
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0

# Add to PATH (add to ~/.bashrc or ~/.profile)
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools' >> ~/.bashrc

# Reload shell
source ~/.bashrc

# Verify installation
dotnet --version
```

## ✅ Verification Steps

### 1. Check .NET Version
```bash
dotnet --version
# Expected output: 9.0.x
```

### 2. Check SDK Information
```bash
dotnet --info
# Should show .NET 9 SDK details
```

### 3. Create Test Project
```bash
# Create test project
dotnet new console -n TestProject
cd TestProject

# Build project
dotnet build

# Run project
dotnet run

# Clean up
cd ..
rm -rf TestProject
```

### 4. Check Available Templates
```bash
dotnet new list
# Should show available project templates
```

## 🔧 Additional Tools

### Entity Framework Tools
```bash
# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

### ASP.NET Core Tools
```bash
# Install ASP.NET Core tools
dotnet tool install --global dotnet-aspnet-codegenerator

# Verify installation
dotnet aspnet-codegenerator --version
```

## 🐛 Troubleshooting

### Issue 1: Command Not Found
```bash
# Check if .NET is in PATH
echo $PATH | grep dotnet

# Add to PATH manually
export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools

# Make permanent (add to shell profile)
echo 'export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools' >> ~/.zshrc
source ~/.zshrc
```

### Issue 2: Permission Denied
```bash
# Fix permissions
sudo chmod +x $(which dotnet)

# Or reinstall with proper permissions
sudo apt-get remove dotnet-sdk-9.0
sudo apt-get install dotnet-sdk-9.0
```

### Issue 3: Version Mismatch
```bash
# Check all installed versions
dotnet --list-sdks

# Use specific version
dotnet --version 9.0.0

# Or set global.json
echo '{"sdk": {"version": "9.0.0"}}' > global.json
```

### Issue 4: SSL Certificate Issues
```bash
# Trust certificates
dotnet dev-certs https --trust

# Or bypass SSL (development only)
export DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
```

## 📚 Next Steps

After successful .NET 9 SDK installation:

1. **Return to Setup Guide**: Continue with [SETUP_GUIDE.md](docs/SETUP_GUIDE.md)
2. **Run Setup Script**: `./scripts/setup.sh`
3. **Start Development**: `dotnet run --project src/HniDashOps.API`

## 🔗 Useful Resources

- **Official .NET 9 Documentation**: https://docs.microsoft.com/en-us/dotnet/core/
- **.NET 9 Release Notes**: https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9
- **Installation Guide**: https://docs.microsoft.com/en-us/dotnet/core/install/
- **Troubleshooting Guide**: https://docs.microsoft.com/en-us/dotnet/core/tools/troubleshoot-usage-issues

---

**Installation Complete! 🎉**

Bây giờ bạn có thể tiếp tục với việc setup HniDashOps Backend project.
