#!/bin/bash
# Apply migration script for CitizenshipApplications module
# Run this from the Api project directory

dotnet ef database update \
  --project ../Mamey.Government.Modules.CitizenshipApplications.Core/Mamey.Government.Modules.CitizenshipApplications.Core.csproj \
  -s ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj \
  --context CitizenshipApplicationsDbContext
