FROM mcr.microsoft.com/dotnet/core/sdk:8.0 AS build
WORKDIR /publish
COPY . .
RUN dotnet publish src/Mamey.Ntrada.Host -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:8.0
WORKDIR /ntrada
COPY --from=build /publish/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT dotnet Mamey.Ntrada.Host.dll