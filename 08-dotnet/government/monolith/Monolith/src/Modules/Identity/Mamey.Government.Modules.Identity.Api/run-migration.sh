#!/bin/bash
# Migration script for Identity module
# Run this from the Api project directory

dotnet ef migrations add Initial \
  --project ../Mamey.Government.Modules.Identity.Core/Mamey.Government.Modules.Identity.Core.csproj \
  -s ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj \
  -o ../Mamey.Government.Modules.Identity.Core/EF/Migrations \
  --context IdentityDbContext
