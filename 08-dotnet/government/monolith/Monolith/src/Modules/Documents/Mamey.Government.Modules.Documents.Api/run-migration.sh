#!/bin/bash
# Migration script for Documents module
# Run this from the Api project directory

dotnet ef migrations add Initial \
  --project ../Mamey.Government.Modules.Documents.Core/Mamey.Government.Modules.Documents.Core.csproj \
  -s ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj \
  -o ../Mamey.Government.Modules.Documents.Core/EF/Migrations \
  --context DocumentsDbContext
