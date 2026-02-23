#!/bin/bash
# Migration script for Notifications module
# Run this from the Api project directory

dotnet ef migrations add Initial \
  --project ../Mamey.Government.Modules.Notifications.Core/Mamey.Government.Modules.Notifications.Core.csproj \
  -s ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj \
  -o ../Mamey.Government.Modules.Notifications.Core/EF/Migrations \
  --context NotificationsDbContext
