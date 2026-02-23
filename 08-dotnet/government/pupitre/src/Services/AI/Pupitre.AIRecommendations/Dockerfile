FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Build Arguments for Azure DevOps NuGet feed
ARG FEED_URL
ARG FEED_PAT
ARG FEED_NAME
ARG FEED_USERNAME

# Set environment variables for the feed
ENV FEED_URL=$FEED_URL
ENV FEED_PAT=$FEED_PAT
ENV FEED_NAME=$FEED_NAME
ENV FEED_USERNAME=$FEED_USERNAME

# Setup the NuGet configuration with credentials for the feed
RUN dotnet nuget add source $FEED_URL \
    --username $FEED_USERNAME --password $FEED_PAT \
    --store-password-in-clear-text --name $FEED_NAME 


# COPY Mamey.Shared /app/Mamey.Shared
COPY Pupitre.AIRecommendations /app/Pupitre.AIRecommendations

# Copy csproj and restore as distinct layers
RUN dotnet restore /app/Pupitre.AIRecommendations

RUN dotnet build /app/Pupitre.AIRecommendations -c release -o /app/out
RUN dotnet publish /app/Pupitre.AIRecommendations -c release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app/Pupitre.AIRecommendations

COPY --from=build /app/out .

ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

ENTRYPOINT ["dotnet", "Pupitre.AIRecommendations.dll"]