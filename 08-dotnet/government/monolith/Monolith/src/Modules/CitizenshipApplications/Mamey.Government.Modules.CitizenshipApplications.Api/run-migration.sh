#!/bin/bash
# Migration script for CitizenshipApplications module
# Run this from the Api project directory

# Build the Core and BlazorServer projects first to ensure DLLs are available
dotnet build ../Mamey.Government.Modules.CitizenshipApplications.Core/Mamey.Government.Modules.CitizenshipApplications.Core.csproj
dotnet build ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj

dotnet ef migrations add Initial \
  --project ../Mamey.Government.Modules.CitizenshipApplications.Core/Mamey.Government.Modules.CitizenshipApplications.Core.csproj \
  -s ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj \
  -o ../Mamey.Government.Modules.CitizenshipApplications.Core/EF/Migrations \
  --context CitizenshipApplicationsDbContext
