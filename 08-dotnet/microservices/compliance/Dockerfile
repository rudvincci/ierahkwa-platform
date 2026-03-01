FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["Mamey.SICB.ZeroKnowledgeProofs/Mamey.SICB.ZeroKnowledgeProofs.csproj", "Mamey.SICB.ZeroKnowledgeProofs/"]
COPY ["Mamey.SICB.TreatyValidators/Mamey.SICB.TreatyValidators.csproj", "Mamey.SICB.TreatyValidators/"]
COPY ["Mamey.SICB.WhistleblowerReports/Mamey.SICB.WhistleblowerReports.csproj", "Mamey.SICB.WhistleblowerReports/"]
RUN dotnet restore "Mamey.SICB.ZeroKnowledgeProofs/Mamey.SICB.ZeroKnowledgeProofs.csproj"
COPY . .
WORKDIR "/src/Mamey.SICB.ZeroKnowledgeProofs"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "Mamey.SICB.ZeroKnowledgeProofs.dll"]
