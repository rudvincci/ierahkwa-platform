/**
 * MCP Server Command
 * 
 * Supports both stdio (for Cursor MCP protocol) and HTTP (standalone) transports
 */

import { McpServer } from '../../server/McpServer';
import { getDashboardServer } from './dashboard';
import * as readline from 'readline';

let mcpServerInstance: McpServer | null = null;

/**
 * Start MCP server in HTTP mode (standalone)
 */
export async function startMcpServer(port: number = 3001, stdio: boolean = false): Promise<void> {
  // If stdio mode, use stdin/stdout for MCP protocol
  if (stdio) {
    await startMcpServerStdio();
    return;
  }

  // HTTP mode - standalone server
  // Try to get dashboard server, but don't fail if it's not available
  // The MCP server can work independently, it just won't have dashboard integration
  // When spawned from WorkflowManager, dashboard server won't be available in this process
  const dashboardServer = getDashboardServer();
  
  mcpServerInstance = new McpServer(port, dashboardServer || undefined);
  
  try {
    await mcpServerInstance.start();
    console.log(`🔌 MCP Server started on http://localhost:${port}`);
    console.log(`   Resources: http://localhost:${port}/mcp/resources`);
    console.log(`   Tools: http://localhost:${port}/mcp/tools`);
    console.log(`   Health: http://localhost:${port}/mcp/health`);
    if (!dashboardServer) {
      console.log(`   ⚠️  Running without dashboard integration (spawned mode)`);
    }
    console.log(`\n💡 To use with Cursor:`);
    console.log(`   1. Copy .cursor/mcp.json.example to .cursor/mcp.json`);
    console.log(`   2. Or configure in Cursor Settings > Features > MCP`);
    console.log(`   3. Use URL: http://127.0.0.1:${port}/mcp`);
    
    // Keep process alive
    process.on('SIGINT', async () => {
      console.log('\n⚠️  SIGINT received. Stopping MCP server...');
      if (mcpServerInstance) {
        await mcpServerInstance.stop();
      }
      process.exit(0);
    });

    process.on('SIGTERM', async () => {
      console.log('\n⚠️  SIGTERM received. Stopping MCP server...');
      if (mcpServerInstance) {
        await mcpServerInstance.stop();
      }
      process.exit(0);
    });

    // Keep process alive
    await new Promise(() => {});
  } catch (error: any) {
    if (error.code === 'EADDRINUSE') {
      console.error(`❌ Port ${port} is already in use. Try a different port with --port`);
    } else {
      console.error('❌ Failed to start MCP server:', error);
    }
    process.exit(1);
  }
}

/**
 * Start MCP server in stdio mode (for Cursor MCP protocol)
 * Reads from stdin, writes to stdout following MCP JSON-RPC protocol
 */
async function startMcpServerStdio(): Promise<void> {
  const { ActivityTracker } = await import('../../services/ActivityTracker');
  const dashboardServer = getDashboardServer();
  const activityTracker = dashboardServer 
    ? (dashboardServer as any).activityTracker 
    : new ActivityTracker();

  // Set up readline for stdin (MCP uses newline-delimited JSON)
  const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
    terminal: false,
  });

  let initialized = false;

  // Handle MCP protocol messages
  rl.on('line', async (line: string) => {
    if (!line.trim()) return;
    
    try {
      const message = JSON.parse(line);
      
      // Handle initialize request
      if (message.method === 'initialize' && !initialized) {
        const initResponse = {
          jsonrpc: '2.0',
          id: message.id,
          result: {
            protocolVersion: '2024-11-05',
            capabilities: {
              tools: {},
              resources: {},
            },
            serverInfo: {
              name: 'maestro-orchestrator',
              version: '1.0.0',
            },
          },
        };
        console.log(JSON.stringify(initResponse));
        initialized = true;
        return;
      }

      // Handle initialized notification
      if (message.method === 'initialized') {
        return; // No response needed
      }

      // Handle other MCP protocol messages
      if (initialized) {
        await handleMcpMessage(message, dashboardServer, activityTracker);
      }
    } catch (error) {
      // Send error response
      const errorResponse = {
        jsonrpc: '2.0',
        id: null,
        error: {
          code: -32700,
          message: 'Parse error',
          data: String(error),
        },
      };
      console.log(JSON.stringify(errorResponse));
    }
  });
}

/**
 * Handle MCP protocol messages
 */
async function handleMcpMessage(
  message: any,
  dashboardServer: any,
  activityTracker: any
): Promise<void> {
  const { method, params, id } = message;

  try {
    let result: any;

    switch (method) {
      case 'tools/list':
        result = {
          tools: [
            {
              name: 'get_workflow_status',
              description: 'Get the current status of a workflow',
              inputSchema: {
                type: 'object',
                properties: {
                  workflowName: { type: 'string', description: 'Name of the workflow' },
                },
                required: ['workflowName'],
              },
            },
            {
              name: 'get_workflow_activity',
              description: 'Get the current activity for a workflow',
              inputSchema: {
                type: 'object',
                properties: {
                  workflowName: { type: 'string', description: 'Name of the workflow' },
                },
                required: ['workflowName'],
              },
            },
            {
              name: 'list_active_workflows',
              description: 'List all currently active workflows',
              inputSchema: {
                type: 'object',
                properties: {},
              },
            },
          ],
        };
        break;

      case 'tools/call':
        result = await handleToolCall(params.name, params.arguments, dashboardServer, activityTracker);
        break;

      case 'resources/list':
        result = {
          resources: [
            {
              uri: 'orchestrator://workflows/active',
              name: 'Active Workflows',
              description: 'List of currently running workflows',
              mimeType: 'application/json',
            },
            {
              uri: 'orchestrator://workflows/:name/status',
              name: 'Workflow Status',
              description: 'Current status of a specific workflow',
              mimeType: 'application/json',
            },
            {
              uri: 'orchestrator://workflows/:name/activity',
              name: 'Workflow Activity',
              description: 'Real-time activity feed for a workflow',
              mimeType: 'application/json',
            },
          ],
        };
        break;

      case 'resources/read':
        result = await handleResourceRead(params.uri, dashboardServer, activityTracker);
        break;

      default:
        throw new Error(`Unknown method: ${method}`);
    }

    const response = {
      jsonrpc: '2.0',
      id,
      result,
    };
    console.log(JSON.stringify(response));
  } catch (error: any) {
    const errorResponse = {
      jsonrpc: '2.0',
      id,
      error: {
        code: -32603,
        message: 'Internal error',
        data: error.message,
      },
    };
    console.log(JSON.stringify(errorResponse));
  }
}

async function handleToolCall(
  toolName: string,
  args: any,
  dashboardServer: any,
  activityTracker: any
): Promise<any> {
  switch (toolName) {
    case 'get_workflow_status':
      if (!dashboardServer) {
        return { error: 'Dashboard server not available' };
      }
      const metrics = await (dashboardServer as any).getMetrics();
      const workflow = metrics.workflows.find((w: any) => w.workflowName === args.workflowName);
      return { content: [{ type: 'text', text: JSON.stringify(workflow || { error: 'Workflow not found' }, null, 2) }] };

    case 'get_workflow_activity':
      const current = activityTracker.getCurrentActivity(args.workflowName);
      const recent = activityTracker.getRecentActivities(args.workflowName, 20);
      return { content: [{ type: 'text', text: JSON.stringify({ current, recent }, null, 2) }] };

    case 'list_active_workflows':
      if (!dashboardServer) {
        return { content: [{ type: 'text', text: JSON.stringify({ workflows: [] }, null, 2) }] };
      }
      const allMetrics = await (dashboardServer as any).getMetrics();
      const active = allMetrics.workflows.filter((w: any) => w.status === 'running');
      return { content: [{ type: 'text', text: JSON.stringify({ workflows: active }, null, 2) }] };

    default:
      throw new Error(`Unknown tool: ${toolName}`);
  }
}

async function handleResourceRead(
  uri: string,
  dashboardServer: any,
  activityTracker: any
): Promise<any> {
  if (uri === 'orchestrator://workflows/active') {
    if (!dashboardServer) {
      return { contents: [] };
    }
    const metrics = await (dashboardServer as any).getMetrics();
    const active = metrics.workflows.filter((w: any) => w.status === 'running');
    return {
      contents: active.map((w: any) => ({
        uri: `orchestrator://workflows/${encodeURIComponent(w.workflowName)}/status`,
        mimeType: 'application/json',
        text: JSON.stringify(w, null, 2),
      })),
    };
  }

  // Handle other resource URIs...
  return { contents: [] };
}

export async function stopMcpServer(): Promise<void> {
  if (mcpServerInstance) {
    await mcpServerInstance.stop();
    mcpServerInstance = null;
  }
}
