# Migration from MameyNode/crates/sdk/mamey-node-sdk

The Rust SDK has been moved from `MameyNode/crates/sdk/mamey-node-sdk` to `MameyNode.Rust/` at the root level for consistency with other SDKs (`MameyNode.Go/`, `MameyNode.JavaScript/`, etc.).

## Changes

1. **Location**: Moved to root level (`MameyNode.Rust/`)
2. **Workspace**: No longer part of MameyNode workspace
3. **Dependencies**: Now uses explicit versions instead of workspace dependencies
4. **Proto Path**: Updated to `../MameyNode/proto` (from `../../../proto`)

## Usage

Update your `Cargo.toml`:

```toml
[dependencies]
# Old (if you were using it from workspace)
# mamey-node-sdk = { path = "../../MameyNode/crates/sdk/mamey-node-sdk" }

# New
mamey-node-sdk = { path = "../MameyNode.Rust" }
```

## Proto Files

The SDK still references proto files from `MameyNode/proto/`. Make sure `MameyNode.Rust/` and `MameyNode/` are siblings in your directory structure:

```
code-final/
├── MameyNode/
│   └── proto/
└── MameyNode.Rust/
    └── build.rs (references ../MameyNode/proto)
```

## Benefits

- **Consistency**: Matches pattern of other SDKs at root level
- **Independence**: Can be versioned and released independently
- **Clarity**: Clear separation between core MameyNode and client SDKs
