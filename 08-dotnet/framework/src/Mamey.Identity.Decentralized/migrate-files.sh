#!/bin/bash

# Bash script to migrate files from Mamey.Auth.DecentralizedIdentifiers to Mamey.Identity.Decentralized
# This script updates namespaces and copies files to the new structure

set -e

SOURCE_DIR="../Mamey.Auth.DecentralizedIdentifiers/src/Mamey.Auth.DecentralizedIdentifiers"
TARGET_DIR="src/Mamey.Identity.Decentralized"

echo "Starting migration from $SOURCE_DIR to $TARGET_DIR"

# Create target directories
mkdir -p "$TARGET_DIR/Abstractions"
mkdir -p "$TARGET_DIR/Configuration"
mkdir -p "$TARGET_DIR/Core"
mkdir -p "$TARGET_DIR/Crypto"
mkdir -p "$TARGET_DIR/Exceptions"
mkdir -p "$TARGET_DIR/Handlers"
mkdir -p "$TARGET_DIR/Methods/Ethr"
mkdir -p "$TARGET_DIR/Methods/Ion"
mkdir -p "$TARGET_DIR/Methods/Key"
mkdir -p "$TARGET_DIR/Methods/MethodBase"
mkdir -p "$TARGET_DIR/Methods/Peer"
mkdir -p "$TARGET_DIR/Methods/Web"
mkdir -p "$TARGET_DIR/Middlewares"
mkdir -p "$TARGET_DIR/Node"
mkdir -p "$TARGET_DIR/Resolution"
mkdir -p "$TARGET_DIR/Resources"
mkdir -p "$TARGET_DIR/Serialization"
mkdir -p "$TARGET_DIR/Services"
mkdir -p "$TARGET_DIR/Trust"
mkdir -p "$TARGET_DIR/Utilities"
mkdir -p "$TARGET_DIR/Validation"
mkdir -p "$TARGET_DIR/VC"

echo "Created target directories"

# Function to update namespace in file content
update_namespace() {
    local file="$1"
    local temp_file=$(mktemp)
    
    # Replace old namespace with new namespace
    sed 's/Mamey\.Auth\.DecentralizedIdentifiers/Mamey.Identity.Decentralized/g' "$file" > "$temp_file"
    
    # Copy the updated content back
    cp "$temp_file" "$file"
    rm "$temp_file"
}

# Copy all .cs files from source to target with namespace updates
echo "Migrating .cs files..."
find "$SOURCE_DIR" -name "*.cs" -type f | while read -r source_file; do
    # Calculate relative path
    relative_path="${source_file#$SOURCE_DIR/}"
    target_file="$TARGET_DIR/$relative_path"
    
    # Create target directory if it doesn't exist
    target_dir=$(dirname "$target_file")
    mkdir -p "$target_dir"
    
    # Copy file
    cp "$source_file" "$target_file"
    
    # Update namespace
    update_namespace "$target_file"
    
    echo "Migrated: $source_file -> $target_file"
done

# Copy other important files
echo "Copying other files..."
if [ -f "$SOURCE_DIR/Resources/did-schema.json" ]; then
    mkdir -p "$TARGET_DIR/Resources"
    cp "$SOURCE_DIR/Resources/did-schema.json" "$TARGET_DIR/Resources/"
    echo "Copied: did-schema.json"
fi

if [ -f "$SOURCE_DIR/Resources/sample-dids/sample-dids.json" ]; then
    mkdir -p "$TARGET_DIR/Resources/sample-dids"
    cp "$SOURCE_DIR/Resources/sample-dids/sample-dids.json" "$TARGET_DIR/Resources/sample-dids/"
    echo "Copied: sample-dids.json"
fi

if [ -f "$SOURCE_DIR/Node/ion-crypto.js" ]; then
    mkdir -p "$TARGET_DIR/Node"
    cp "$SOURCE_DIR/Node/ion-crypto.js" "$TARGET_DIR/Node/"
    echo "Copied: ion-crypto.js"
fi

echo "Migration completed successfully!"
echo "Files migrated to: $TARGET_DIR"
