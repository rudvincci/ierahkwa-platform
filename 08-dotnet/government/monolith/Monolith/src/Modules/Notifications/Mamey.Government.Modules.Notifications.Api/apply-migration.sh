#!/bin/bash
# Apply migration script for Notifications module
# Run this from the Api project directory

dotnet ef database update \
  --project ../Mamey.Government.Modules.Notifications.Core/Mamey.Government.Modules.Notifications.Core.csproj \
  -s ../../../Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj \
  --context NotificationsDbContext
