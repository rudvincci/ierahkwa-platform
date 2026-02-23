#!/bin/bash

echo "Adding portal and service projects..."

# Portals and Gateways
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenPortal/Mamey.Government.CitizenPortal/Mamey.Government.CitizenPortal.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenPortal/Mamey.Government.CitizenPortal.Client/Mamey.Government.CitizenPortal.Client.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.CitizenPortal.ApiGataway/src/Mamey.Government.CitizenPortal.ApiGataway/Mamey.Government.CitizenPortal.ApiGataway.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.GovernmentPortal.ApiGateway/src/Mamey.Government.GovernmentPortal.ApiGateway/Mamey.Government.GovernmentPortal.ApiGateway.csproj

# Notifications
dotnet sln Mamey.Government.sln add ../Mamey.Government.Notifications/src/Mamey.Government.Notifications/Mamey.Government.Notifications.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Notifications/src/Mamey.Government.Notifications.Application/Mamey.Government.Notifications.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Notifications/src/Mamey.Government.Notifications.Domain/Mamey.Government.Notifications.Domain.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Notifications/src/Mamey.Government.Notifications.Infrastructure/Mamey.Government.Notifications.Infrastructure.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Notifications/src/Mamey.Government.Notifications.BlazorWasm/Mamey.Government.Notifications.BlazorWasm.csproj

# Operations
dotnet sln Mamey.Government.sln add ../Mamey.Government.Operations/src/Mamey.Government.Operations.Api/Mamey.Government.Operations.Api.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.Operations/src/Mamey.Government.Operations.GrpcClient/Mamey.Government.Operations.GrpcClient.csproj

# Payment Service Saga
dotnet sln Mamey.Government.sln add ../Mamey.Government.PaymentService.Saga/src/Mamey.Government.PaymentService.Saga.Api/Mamey.Government.PaymentService.Saga.Api.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.PaymentService.Saga/src/Mamey.Government.PaymentService.Saga.Application/Mamey.Government.PaymentService.Saga.Application.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.PaymentService.Saga/src/Mamey.Government.PaymentService.Saga.Domain/Mamey.Government.PaymentService.Saga.Domain.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.PaymentService.Saga/src/Mamey.Government.PaymentService.Saga.Contracts/Mamey.Government.PaymentService.Saga.Contracts.csproj
dotnet sln Mamey.Government.sln add ../Mamey.Government.PaymentService.Saga/src/Mamey.Government.PaymentService.Saga.BlazorWasm/Mamey.Government.PaymentService.Saga.BlazorWasm.csproj

echo "Added portal and service projects..."
