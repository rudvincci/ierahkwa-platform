# MCP Server Setup Guide

The Maestro MCP server supports **both** Cursor's MCP protocol and standalone HTTP access.

## Transport Modes

### 1. HTTP Mode (Standalone)
Use this for direct API access or when running as a standalone server.

```bash
maestro mcp --port 3001
```

Access endpoints:
- Resources: `http://localhost:3001/mcp/resources`
- Tools: `http://localhost:3001/mcp/tools`
- Health: `http://localhost:3001/mcp/health`

### 2. stdio Mode (Cursor MCP Protocol)
Use this for integration with Cursor IDE via MCP protocol.

```bash
maestro mcp --stdio
```

## Cursor Configuration

### Option 1: Using mcp.json (Recommended)

1. Copy the example configuration:
```bash
cp .cursor/mcp.json.example .cursor/mcp.json
```

2. Edit `.cursor/mcp.json` to match your setup:

**For stdio transport (Cursor manages the process):**
```json
{
  "mcpServers": {
    "maestro-orchestrator": {
      "command": "node",
      "args": [
        ".maestro/dist/cli/index.js",
        "mcp",
        "--stdio"
      ],
      "env": {}
    }
  }
}
```

**For HTTP transport (standalone server):**
```json
{
  "mcpServers": {
    "maestro-orchestrator-http": {
      "url": "http://127.0.0.1:3001/mcp"
    }
  }
}
```

### Option 2: Using Cursor Settings UI

1. Open Cursor Settings
2. Navigate to `Features` > `MCP`
3. Click `+ Add New MCP Server`
4. Choose transport:
   - **stdio**: Command: `node`, Args: `.maestro/dist/cli/index.js mcp --stdio`
   - **HTTP**: URL: `http://127.0.0.1:3001/mcp`

## Verification

### Check MCP Server Status
```bash
# List configured MCP servers in Cursor
cursor-agent mcp list

# List tools from Maestro MCP server
cursor-agent mcp list-tools maestro-orchestrator
```

### Test HTTP Endpoints
```bash
# Check health
curl http://localhost:3001/mcp/health

# List resources
curl http://localhost:3001/mcp/resources

# List tools
curl http://localhost:3001/mcp/tools
```

## Available MCP Tools

1. **get_workflow_status** - Get current status of a workflow
2. **get_workflow_activity** - Get activity feed for a workflow
3. **list_active_workflows** - List all running workflows

## Available MCP Resources

1. **orchestrator://workflows/active** - List of active workflows
2. **orchestrator://workflows/:name/status** - Workflow status
3. **orchestrator://workflows/:name/activity** - Workflow activity feed

## Troubleshooting

### MCP Server Not Appearing in Cursor

1. Ensure the server is running:
   ```bash
   maestro mcp --stdio  # For stdio mode
   # OR
   maestro mcp          # For HTTP mode
   ```

2. Check `.cursor/mcp.json` syntax is valid JSON

3. Restart Cursor after configuration changes

4. Check Cursor logs for MCP connection errors

### Port Already in Use

If port 3001 is already in use:
```bash
maestro mcp --port 3002  # Use different port
```

Update `.cursor/mcp.json` with the new port if using HTTP transport.

### stdio Mode Not Working

- Ensure Node.js is in your PATH
- Verify the path to `.maestro/dist/cli/index.js` is correct
- Check that the project has been built: `npm run build`

## Integration with maestro enable/disable

- `maestro enable` automatically starts the MCP server in HTTP mode on port 3001
- `maestro disable` automatically stops the MCP server
- For stdio mode with Cursor, you need to configure it separately in `.cursor/mcp.json`

