# Mamey.Tools.Cli

Hardhat-style developer CLI for MameyNode.

## Status

Scaffolded (command routing + config loader + proto sanity runner).  
`grpc smoke` is intentionally a stub until the minimal staging gRPC surface (Gate C) is green.

## Run (from repo)

```bash
dotnet run --project Mamey/src/Mamey.Tools.Cli/src/Mamey.Tools.Cli/Mamey.Tools.Cli.csproj -- --help
```

## Commands (scaffold)

- `mamey init`
- `mamey protos check`
- `mamey grpc smoke` *(stub)*




