#!/bin/bash
# Migration script for TravelIdentities module
# Run this from the Api project directory

# Build the Core and BlazorServer projects first to ensure DLLs are available
dotnet build ../Mamey.Government.Modules.TravelIdentities.Core/Mamey.Government.Modules.TravelIdentities.Core.csproj
dotnet build ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj

dotnet ef migrations add Initial \
  --project ../Mamey.Government.Modules.TravelIdentities.Core/Mamey.Government.Modules.TravelIdentities.Core.csproj \
  -s ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj \
  -o ../Mamey.Government.Modules.TravelIdentities.Core/EF/Migrations \
  --context TravelIdentitiesDbContext
