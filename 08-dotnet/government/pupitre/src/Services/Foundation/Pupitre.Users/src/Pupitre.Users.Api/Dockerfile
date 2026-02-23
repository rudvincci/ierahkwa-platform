FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Api/Pupitre.Users.Api.csproj", "src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Api/"]
COPY ["src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Application/Pupitre.Users.Application.csproj", "src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Application/"]
COPY ["src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Domain/Pupitre.Users.Domain.csproj", "src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Domain/"]
COPY ["src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Infrastructure/Pupitre.Users.Infrastructure.csproj", "src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Infrastructure/"]
COPY ["src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Contracts/Pupitre.Users.Contracts.csproj", "src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Contracts/"]
COPY ["src/Shared/Pupitre.Types/Pupitre.Types.csproj", "src/Shared/Pupitre.Types/"]
COPY ["src/Shared/Pupitre.Contracts/Pupitre.Contracts.csproj", "src/Shared/Pupitre.Contracts/"]
COPY ["src/Shared/Pupitre.Exceptions/Pupitre.Exceptions.csproj", "src/Shared/Pupitre.Exceptions/"]

RUN dotnet restore "src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Api/Pupitre.Users.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Api"
RUN dotnet build "Pupitre.Users.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.Users.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60001
ENTRYPOINT ["dotnet", "Pupitre.Users.Api.dll"]
