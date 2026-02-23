#!/bin/bash

echo "Adding final Mamey.Government projects..."

# Passports
dotnet sln Mamey.Government.sln add ../Mamey.Government.Passports/src/Mamey.Government.Passports/Mamey.Government.Passports.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Passports/src/Mamey.Government.Passports.Application/Mamey.Government.Passports.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Passports/src/Mamey.Government.Passports.Domain/Mamey.Government.Passports.Domain.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Passports/src/Mamey.Government.Passports.Infrastructure/Mamey.Government.Passports.Infrastructure.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Passports/src/Mamey.Government.Passports.BlazorWasm/Mamey.Government.Passports.BlazorWasm.csproj

# Passport Management Saga
dotnet sln Mamey.Government.sln add ../Mamey.Government.PassportManagement.Saga/src/Mamey.Government.PassportManagement.Saga.Application/Mamey.Government.PassportManagement.Saga.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.PassportManagement.Saga/src/Mamey.Government.PassportManagement.Saga.Domain/Mamey.Government.PassportManagement.Saga.Domain.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.PassportManagement.Saga/src/Mamey.Government.PassportManagement.Saga.BlazorWasm/Mamey.Government.PassportManagement.Saga.BlazorWasm.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.PassportManagement.Saga/src/Mamey.Government.PassportManagement.Saga.Net/Mamey.Government.PassportManagement.Saga.Net.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.PassportManagement.Saga/src/Mamey.Government.PassportManagement.Saga.Maui/Mamey.Government.PassportManagement.Saga.Maui.csproj

# Diplomats
dotnet sln Mamey.Government.sln add ../Mamey.Government.Diplomats/src/Mamey.Government.Diplomats/Mamey.Government.Diplomats.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Diplomats/src/Mamey.Government.Diplomats.Application/Mamey.Government.Diplomats.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Diplomats/src/Mamey.Government.Diplomats.Domain/Mamey.Government.Diplomats.Domain.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Diplomats/src/Mamey.Government.Diplomats.Infrastructure/Mamey.Government.Diplomats.Infrastructure.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Diplomats/src/Mamey.Government.Diplomats.BlazorWasm/Mamey.Government.Diplomats.BlazorWasm.csproj

# Diplomat Management Saga
dotnet sln Mamey.Government.sln add ../Mamey.Government.DiplomatManagement.Saga/src/Mamey.Government.DiplomatManagement.Saga.Application/Mamey.Government.DiplomatManagement.Saga.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.DiplomatManagement.Saga/src/Mamey.Government.DiplomatManagement.Saga.Contracts/Mamey.Government.DiplomatManagement.Saga.Contracts.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.DiplomatManagement.Saga/src/Mamey.Government.DiplomatManagement.Saga.Infrastructure/Mamey.Government.DiplomatManagement.Saga.Infrastructure.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.DiplomatManagement.Saga/src/Mamey.Government.DiplomatManagement.Saga.BlazorWasm/Mamey.Government.DiplomatManagement.Saga.BlazorWasm.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.DiplomatManagement.Saga/src/Mamey.Government.DiplomatManagement.Saga.Net/Mamey.Government.DiplomatManagement.Saga.Net.csproj

echo "Added passport and diplomat projects..."
