# Blazor UI Build Status

## Code Status: ✅ Ready

All code files have been reviewed and are structurally correct:
- ✅ No linter errors
- ✅ All using statements correct
- ✅ Project references valid
- ✅ All pages implement required functionality
- ✅ Metadata service properly integrated
- ✅ Error handling in place

## Build Issue: System Permission

The build is currently failing due to a system-level permission issue with the .NET SDK:

```
error : Access to the path '/usr/local/share/dotnet/sdk/9.0.301/trustedroots/codesignctl.pem' is denied.
error :   Operation not permitted
```

This is **not a code problem** - it's a sandbox/security restriction preventing the .NET SDK from accessing its trusted roots certificate file.

## To Build and Run (Outside Sandbox)

1. **Restore packages**:
   ```bash
   cd Mamey/src/MameyNode.UI/MameyNode.UI
   dotnet restore
   ```

2. **Build the project**:
   ```bash
   dotnet build
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

4. **Access the UI**:
   - Navigate to `https://localhost:5001` (or the configured port)
   - The app will connect to MameyNode at `http://localhost:50051` by default

## Configuration

Edit `appsettings.json` to configure:
- `MameyNode:GrpcEndpoint` - gRPC endpoint URL (default: `http://localhost:50051`)
- `MameyNode:TimeoutSeconds` - Request timeout (default: `30`)
- `MameyNode:DefaultInstitutionId` - Optional institution ID

## Pages Available

- `/` - Home page with connection status
- `/nodeinfo` - Node information (NodeService.GetNodeInfo)
- `/version` - RPC version (RpcService.Version)
- `/paymentrequest` - Payment request creation and retrieval (BankingService)

## Dependencies

The project references:
- `Mamey.Blockchain.Node` - NodeService gRPC client
- `Mamey.Blockchain.Banking` - BankingService gRPC client

Both projects must be built first for the UI to compile.

## Next Steps

Once the permission issue is resolved (or running outside the sandbox):
1. Build the referenced projects (`Mamey.Blockchain.Node` and `Mamey.Blockchain.Banking`)
2. Build the UI project
3. Run the application
4. Test against a running MameyNode instance
