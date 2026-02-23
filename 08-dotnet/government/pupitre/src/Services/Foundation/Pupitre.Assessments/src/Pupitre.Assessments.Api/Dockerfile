FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60005

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Api/Pupitre.Assessments.Api.csproj", "src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Api/"]
COPY ["src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Application/Pupitre.Assessments.Application.csproj", "src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Application/"]
COPY ["src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Domain/Pupitre.Assessments.Domain.csproj", "src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Domain/"]
COPY ["src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Infrastructure/Pupitre.Assessments.Infrastructure.csproj", "src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Infrastructure/"]
COPY ["src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Contracts/Pupitre.Assessments.Contracts.csproj", "src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Contracts/"]
COPY ["src/Shared/Pupitre.Types/Pupitre.Types.csproj", "src/Shared/Pupitre.Types/"]
COPY ["src/Shared/Pupitre.Contracts/Pupitre.Contracts.csproj", "src/Shared/Pupitre.Contracts/"]
COPY ["src/Shared/Pupitre.Exceptions/Pupitre.Exceptions.csproj", "src/Shared/Pupitre.Exceptions/"]

RUN dotnet restore "src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Api/Pupitre.Assessments.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Foundation/Pupitre.Assessments/src/Pupitre.Assessments.Api"
RUN dotnet build "Pupitre.Assessments.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.Assessments.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60005
ENTRYPOINT ["dotnet", "Pupitre.Assessments.Api.dll"]
