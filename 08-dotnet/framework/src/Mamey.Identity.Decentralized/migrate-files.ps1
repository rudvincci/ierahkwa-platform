# PowerShell script to migrate files from Mamey.Auth.DecentralizedIdentifiers to Mamey.Identity.Decentralized
# This script updates namespaces and copies files to the new structure

$sourceDir = "../Mamey.Auth.DecentralizedIdentifiers/src/Mamey.Auth.DecentralizedIdentifiers"
$targetDir = "src/Mamey.Identity.Decentralized"

# Create target directories
$directories = @(
    "Abstractions",
    "Configuration", 
    "Core",
    "Crypto",
    "Exceptions",
    "Handlers",
    "Methods",
    "Methods/Ethr",
    "Methods/Ion", 
    "Methods/Key",
    "Methods/MethodBase",
    "Methods/Peer",
    "Methods/Web",
    "Middlewares",
    "Node",
    "Resolution",
    "Resources",
    "Serialization",
    "Services",
    "Trust",
    "Utilities",
    "Validation",
    "VC"
)

foreach ($dir in $directories) {
    $fullPath = Join-Path $targetDir $dir
    if (!(Test-Path $fullPath)) {
        New-Item -ItemType Directory -Path $fullPath -Force
        Write-Host "Created directory: $fullPath"
    }
}

# Function to update namespace in file content
function Update-Namespace {
    param($content)
    
    # Replace old namespace with new namespace
    $content = $content -replace "Mamey\.Auth\.DecentralizedIdentifiers", "Mamey.Identity.Decentralized"
    
    return $content
}

# Function to copy and update file
function Copy-AndUpdateFile {
    param($sourceFile, $targetFile)
    
    if (Test-Path $sourceFile) {
        $content = Get-Content $sourceFile -Raw
        $updatedContent = Update-Namespace $content
        
        # Ensure target directory exists
        $targetDir = Split-Path $targetFile -Parent
        if (!(Test-Path $targetDir)) {
            New-Item -ItemType Directory -Path $targetDir -Force
        }
        
        Set-Content -Path $targetFile -Value $updatedContent -Encoding UTF8
        Write-Host "Migrated: $sourceFile -> $targetFile"
    }
}

# Copy all .cs files from source to target with namespace updates
Get-ChildItem -Path $sourceDir -Recurse -Filter "*.cs" | ForEach-Object {
    $relativePath = $_.FullName.Substring($sourceDir.Length + 1)
    $targetFile = Join-Path $targetDir $relativePath
    
    Copy-AndUpdateFile $_.FullName $targetFile
}

# Copy other important files
$otherFiles = @(
    "Resources/did-schema.json",
    "Resources/sample-dids/sample-dids.json",
    "Node/ion-crypto.js"
)

foreach ($file in $otherFiles) {
    $sourceFile = Join-Path $sourceDir $file
    $targetFile = Join-Path $targetDir $file
    
    if (Test-Path $sourceFile) {
        $targetDir = Split-Path $targetFile -Parent
        if (!(Test-Path $targetDir)) {
            New-Item -ItemType Directory -Path $targetDir -Force
        }
        
        Copy-Item $sourceFile $targetFile -Force
        Write-Host "Copied: $sourceFile -> $targetFile"
    }
}

Write-Host "Migration completed successfully!"
Write-Host "Files migrated to: $targetDir"
