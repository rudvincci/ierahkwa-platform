#!/bin/bash
# Migration script for Passports module
# Run this from the Api project directory

# Build the Core and BlazorServer projects first to ensure DLLs are available
dotnet build ../Mamey.Government.Modules.Passports.Core/Mamey.Government.Modules.Passports.Core.csproj
dotnet build ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj

dotnet ef migrations add Initial \
  --project ../Mamey.Government.Modules.Passports.Core/Mamey.Government.Modules.Passports.Core.csproj \
  -s ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj \
  -o ../Mamey.Government.Modules.Passports.Core/EF/Migrations \
  --context PassportsDbContext
