#!/bin/bash

# Add all Mamey.Government projects to the solution
echo "Adding Mamey.Government projects to solution..."

# Shared
dotnet sln Mamey.Government.sln add ../Mamey.Government.Shared/tests/Mamey.Government.Shared.Tests/Mamey.Government.Shared.Tests.csproj

# Citizen Management
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement/src/Mamey.Government.CitizenManagement.Api/Mamey.Government.CitizenManagement.Api.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement/src/Mamey.Government.CitizenManagement.Application/Mamey.Government.CitizenManagement.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement/src/Mamey.Government.CitizenManagement.Domain/Mamey.Government.CitizenManagement.Domain.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement/src/Mamey.Government.CitizenManagement.Infrastructure/Mamey.Government.CitizenManagement.Infrastructure.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement/src/Mamey.Government.CitizenManagement.Contracts/Mamey.Government.CitizenManagement.Contracts.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement/src/Mamey.Government.CitizenManagement.Maui/Mamey.Government.CitizenManagement.Maui.csproj

# Citizen Management Saga
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement.Saga/src/Mamey.Government.CitizenManagement.Saga.Api/Mamey.Government.CitizenManagement.Saga.Api.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement.Saga/src/Mamey.Government.CitizenManagement.Saga.Application/Mamey.Government.CitizenManagement.Saga.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement.Saga/src/Mamey.Government.CitizenManagement.Saga.Domain/Mamey.Government.CitizenManagement.Saga.Domain.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement.Saga/src/Mamey.Government.CitizenManagement.Saga.Infrastructure/Mamey.Government.CitizenManagement.Saga.Infrastructure.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement.Saga/src/Mamey.Government.CitizenManagement.Saga.Contracts/Mamey.Government.CitizenManagement.Saga.Contracts.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenManagement.Saga/src/Mamey.Government.CitizenManagement.Saga.Maui/Mamey.Government.CitizenManagement.Saga.Maui.csproj

echo "Added core citizen management projects..."
