# escape=`

# Build stage
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS builder

WORKDIR /src

# Copy project files for dependency restoration
COPY Tour_Management.csproj .
COPY packages.config .

# Restore NuGet packages
RUN nuget restore Tour_Management.csproj

# Copy all source files
COPY . .

# Build the application
RUN msbuild Tour_Management.csproj /p:Configuration=Release /p:Platform=AnyCPU /p:DeployOnBuild=true /p:PublishProfile=FolderProfile /p:OutputPath=C:\publish\

# Runtime stage
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8

WORKDIR /inetpub/wwwroot

# Copy published output from builder stage
COPY --from=builder /publish/_PublishedWebsites/Tour_Management .

# Set environment variables for connection string (override in deployment)
ENV DB_CONNECTION_STRING="Data Source=sql-server;Initial Catalog=tourdb;User ID=sa;Password=YourPassword123;MultipleActiveResultSets=true"

# Expose HTTP port
EXPOSE 80

# Configure IIS to use environment variable for connection string
RUN powershell -Command `
    Import-Module WebAdministration; `
    Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name physicalPath -Value 'C:\inetpub\wwwroot'

# No CMD needed - base image handles IIS startup