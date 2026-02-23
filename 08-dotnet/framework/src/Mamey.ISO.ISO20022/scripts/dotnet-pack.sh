#!/bin/bash
echo "Executing after success scripts on branch $GITHUB_REF_NAME"
echo "Triggering package build"

cd src/Mamey.ISO.ISO20022/src/Mamey.ISO.ISO20022

dotnet pack -c release /p:PackageVersion=1.1.$GITHUB_RUN_NUMBER --no-restore -o .

echo "Uploading package to Azure Artifacts using branch $GITHUB_REF_NAME"

# Assuming Azure DevOps details are set as environment variables or hardcoded
# AZURE_DEVOPS_FEED_URL is the URL to your Azure Artifacts feed
# AZURE_DEVOPS_PAT is your Personal Access Token
case "$GITHUB_REF_NAME" in
  "master")
    dotnet nuget push "*.nupkg" --api-key $AZURE_DEVOPS_PAT --source "$AZURE_DEVOPS_FEED_URL"
    ;;
esac