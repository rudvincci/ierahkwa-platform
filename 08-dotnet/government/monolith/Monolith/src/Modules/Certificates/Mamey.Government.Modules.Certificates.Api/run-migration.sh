#!/bin/bash
# Migration script for Certificates module
# Run this from the Api project directory

dotnet ef migrations add Initial \
  --project ../Mamey.Government.Modules.Certificates.Core/Mamey.Government.Modules.Certificates.Core.csproj \
  -s ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj \
  -o ../Mamey.Government.Modules.Certificates.Core/EF/Migrations \
  --context CertificatesDbContext
