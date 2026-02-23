#!/bin/bash
# Sync proto files from MameyNode to .NET SDK
# This script should be run from the Mamey repository root

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../../../../.." && pwd)"
MANEYNODE_PROTO_DIR="$REPO_ROOT/MameyNode/proto"
SDK_PROTO_DIR="$SCRIPT_DIR/../src/Mamey.Blockchain.Node/Protos"

echo "Syncing proto files from MameyNode to .NET SDK..."
echo "Source: $MANEYNODE_PROTO_DIR"
echo "Destination: $SDK_PROTO_DIR"

# Check if source directory exists
if [ ! -d "$MANEYNODE_PROTO_DIR" ]; then
    echo "Error: MameyNode proto directory not found: $MANEYNODE_PROTO_DIR"
    exit 1
fi

# Check if destination directory exists
if [ ! -d "$SDK_PROTO_DIR" ]; then
    echo "Error: SDK proto directory not found: $SDK_PROTO_DIR"
    exit 1
fi

# Sync node.proto
if [ -f "$MANEYNODE_PROTO_DIR/node.proto" ]; then
    echo "Syncing node.proto..."
    cp "$MANEYNODE_PROTO_DIR/node.proto" "$SDK_PROTO_DIR/node.proto"
    echo "✓ node.proto synced"
else
    echo "Warning: node.proto not found in MameyNode"
fi

echo "Proto sync complete!"
