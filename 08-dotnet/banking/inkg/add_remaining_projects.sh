#!/bin/bash

echo "Adding remaining Mamey.Government projects..."

# Citizens
dotnet sln Mamey.Government.sln add ../Mamey.Government.Citizens/src/Mamey.Government.Citizens/Mamey.Government.Citizens.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Citizens/src/Mamey.Government.Citizens.Application/Mamey.Government.Citizens.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Citizens/src/Mamey.Government.Citizens.Domain/Mamey.Government.Citizens.Domain.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Citizens/src/Mamey.Government.Citizens.Infrastructure/Mamey.Government.Citizens.Infrastructure.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Citizens/src/Mamey.Government.Citizens.BlazorWasm/Mamey.Government.Citizens.BlazorWasm.csproj

# Citizenship Applications
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplications/src/Mamey.Government.CitizenshipApplications/Mamey.Government.CitizenshipApplications.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplications/src/Mamey.Government.CitizenshipApplications.Application/Mamey.Government.CitizenshipApplications.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplications/src/Mamey.Government.CitizenshipApplications.Domain/Mamey.Government.CitizenshipApplications.Domain.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplications/src/Mamey.Government.CitizenshipApplications.Infrastructure/Mamey.Government.CitizenshipApplications.Infrastructure.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplications/src/Mamey.Government.CitizenshipApplications.Contracts/Mamey.Government.CitizenshipApplications.Contracts.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplications/src/Mamey.Government.CitizenshipApplications.Core/Mamey.Government.CitizenshipApplications.Core.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplications/src/Mamey.Government.CitizenshipApplications.Net/Mamey.Government.CitizenshipApplications.Net.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplications/src/Mamey.Government.CitizenshipApplications.Maui/Mamey.Government.CitizenshipApplications.Maui.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplications/src/Mamey.Government.CitizenshipApplications.BlazorWasm/Mamey.Government.CitizenshipApplications.BlazorWasm.csproj

# Citizenship Application Management
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplicationManagement/src/Mamey.Government.CitizenshipApplicationManagement.Api/Mamey.Government.CitizenshipApplicationManagement.Api.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplicationManagement/src/Mamey.Government.CitizenshipApplicationManagement.Application/Mamey.Government.CitizenshipApplicationManagement.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplicationManagement/src/Mamey.Government.CitizenshipApplicationManagement.Domain/Mamey.Government.CitizenshipApplicationManagement.Domain.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplicationManagement/src/Mamey.Government.CitizenshipApplicationManagement.Infrastructure/Mamey.Government.CitizenshipApplicationManagement.Infrastructure.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenshipApplicationManagement/src/Mamey.Government.CitizenshipApplicationManagement.BlazorWasm/Mamey.Government.CitizenshipApplicationManagement.BlazorWasm.csproj

echo "Added citizenship-related projects..."
