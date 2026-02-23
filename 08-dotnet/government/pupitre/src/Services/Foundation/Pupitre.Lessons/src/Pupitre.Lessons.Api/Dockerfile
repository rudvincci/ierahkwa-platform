FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60004

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Api/Pupitre.Lessons.Api.csproj", "src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Api/"]
COPY ["src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Application/Pupitre.Lessons.Application.csproj", "src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Application/"]
COPY ["src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Domain/Pupitre.Lessons.Domain.csproj", "src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Domain/"]
COPY ["src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Infrastructure/Pupitre.Lessons.Infrastructure.csproj", "src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Infrastructure/"]
COPY ["src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Contracts/Pupitre.Lessons.Contracts.csproj", "src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Contracts/"]
COPY ["src/Shared/Pupitre.Types/Pupitre.Types.csproj", "src/Shared/Pupitre.Types/"]
COPY ["src/Shared/Pupitre.Contracts/Pupitre.Contracts.csproj", "src/Shared/Pupitre.Contracts/"]
COPY ["src/Shared/Pupitre.Exceptions/Pupitre.Exceptions.csproj", "src/Shared/Pupitre.Exceptions/"]

RUN dotnet restore "src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Api/Pupitre.Lessons.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Foundation/Pupitre.Lessons/src/Pupitre.Lessons.Api"
RUN dotnet build "Pupitre.Lessons.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.Lessons.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60004
ENTRYPOINT ["dotnet", "Pupitre.Lessons.Api.dll"]
