# PowerShell script to generate Authentik API client from OpenAPI schema
param(
    [Parameter(Mandatory=$true)]
    [string]$AuthentikUrl,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath = "src/Mamey.Authentik.Generated",
    
    [Parameter(Mandatory=$false)]
    [string]$SchemaPath = "schema.json"
)

Write-Host "Fetching OpenAPI schema from $AuthentikUrl/api/v3/schema/" -ForegroundColor Green

# Fetch schema
$schemaUrl = "$AuthentikUrl/api/v3/schema/?format=json"
try {
    $response = Invoke-WebRequest -Uri $schemaUrl -Method Get
    $schemaContent = $response.Content
    $schemaContent | Out-File -FilePath $SchemaPath -Encoding UTF8
    Write-Host "Schema saved to $SchemaPath" -ForegroundColor Green
} catch {
    Write-Host "Error fetching schema: $_" -ForegroundColor Red
    exit 1
}

Write-Host "Generating client code using NSwag..." -ForegroundColor Green

# Check if NSwag is installed
$nswagPath = Get-Command nswag -ErrorAction SilentlyContinue
if (-not $nswagPath) {
    Write-Host "NSwag CLI not found. Installing..." -ForegroundColor Yellow
    dotnet tool install -g NSwag.ConsoleCore
}

# Generate client
nswag openapi2csclient /input:$SchemaPath /output:$OutputPath /namespace:Mamey.Authentik.Generated /className:AuthentikClient /generateClientClasses:true /generateClientInterfaces:true /useHttpClientCreationMethod:true /useHttpRequestMessageCreationMethod:true /useBaseUrl:true /generateExceptionClasses:true /exceptionClass:AuthentikApiException /jsonLibrary:SystemTextJson /dateType:System.DateTimeOffset /generateOptionalParameters:true /generateJsonMethods:false /useTransformOptionsMethod:true /useTransformResultMethod:true /targetFramework:net9.0

Write-Host "Client generation complete!" -ForegroundColor Green
