/**
 * MCP (Model Context Protocol) Server
 * 
 * Exposes orchestrator state and operations via MCP protocol
 */

import * as http from 'http';
import express, { Request, Response } from 'express';
import { DashboardServer } from './DashboardServer';
import { ActivityTracker } from '../services/ActivityTracker';

export interface McpResource {
  uri: string;
  name: string;
  description: string;
  mimeType?: string;
}

export interface McpTool {
  name: string;
  description: string;
  inputSchema: {
    type: 'object';
    properties: Record<string, any>;
    required?: string[];
  };
}

export class McpServer {
  private app: express.Application;
  private server: http.Server;
  private port: number;
  private dashboardServer?: DashboardServer;
  private activityTracker: ActivityTracker;

  constructor(port: number = 3001, dashboardServer?: DashboardServer) {
    this.port = port;
    this.dashboardServer = dashboardServer;
    // Create activity tracker if dashboard server is available, otherwise create standalone
    this.activityTracker = dashboardServer 
      ? (dashboardServer as any).activityTracker 
      : new ActivityTracker();
    this.app = express();
    this.server = http.createServer(this.app);
    
    this.setupRoutes();
  }

  private setupRoutes(): void {
    this.app.use(express.json());
    
    // MCP Protocol endpoints
    this.app.get('/mcp/resources', (req, res) => {
      const resources: McpResource[] = [
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
        {
          uri: 'orchestrator://workflows/:name/steps',
          name: 'Workflow Steps',
          description: 'List of steps in a workflow',
          mimeType: 'application/json',
        },
        {
          uri: 'orchestrator://system/info',
          name: 'System Information',
          description: 'System resource and performance metrics',
          mimeType: 'application/json',
        },
        {
          uri: 'orchestrator://memory/search',
          name: 'Memory Search',
          description: 'Search memory entries',
          mimeType: 'application/json',
        },
        {
          uri: 'orchestrator://cursor/project',
          name: 'Cursor Project',
          description: 'Cursor project information and statistics',
          mimeType: 'application/json',
        },
      ];
      res.json({ resources });
    });

    // Get active workflows
    this.app.get('/mcp/resources/orchestrator://workflows/active', async (req, res) => {
      try {
        if (!this.dashboardServer) {
          res.json({ contents: [] });
          return;
        }
        const metrics = await (this.dashboardServer as any).getMetrics();
        const active = metrics.workflows.filter((w: any) => w.status === 'running');
        res.json({
          contents: active.map((w: any) => ({
            uri: `orchestrator://workflows/${encodeURIComponent(w.workflowName)}/status`,
            mimeType: 'application/json',
            text: JSON.stringify(w, null, 2),
          })),
        });
      } catch (error) {
        res.status(500).json({ error: String(error) });
      }
    });

    // Get workflow status
    this.app.get('/mcp/resources/orchestrator://workflows/:name/status', async (req, res) => {
      try {
        if (!this.dashboardServer) {
          res.status(404).json({ error: 'Dashboard server not available' });
          return;
        }
        const workflowName = decodeURIComponent(req.params.name);
        const metrics = await (this.dashboardServer as any).getMetrics();
        const workflow = metrics.workflows.find((w: any) => w.workflowName === workflowName);
        
        if (!workflow) {
          res.status(404).json({ error: 'Workflow not found' });
          return;
        }

        res.json({
          contents: [{
            uri: `orchestrator://workflows/${encodeURIComponent(workflowName)}/status`,
            mimeType: 'application/json',
            text: JSON.stringify(workflow, null, 2),
          }],
        });
      } catch (error) {
        res.status(500).json({ error: String(error) });
      }
    });

    // Get workflow activity
    this.app.get('/mcp/resources/orchestrator://workflows/:name/activity', async (req, res) => {
      try {
        const workflowName = decodeURIComponent(req.params.name);
        // Activity tracker works independently, doesn't need dashboard server
        const current = this.activityTracker.getCurrentActivity(workflowName);
        const recent = this.activityTracker.getRecentActivities(workflowName, 50);
        
        res.json({
          contents: [{
            uri: `orchestrator://workflows/${encodeURIComponent(workflowName)}/activity`,
            mimeType: 'application/json',
            text: JSON.stringify({ current, recent }, null, 2),
          }],
        });
      } catch (error) {
        res.status(500).json({ error: String(error) });
      }
    });

    // Get system information
    this.app.get('/mcp/resources/orchestrator://system/info', async (req, res) => {
      try {
        if (!this.dashboardServer) {
          res.status(503).json({ error: 'Dashboard server not available' });
          return;
        }
        const systemInfoService = (this.dashboardServer as any).systemInfoService;
        if (!systemInfoService) {
          res.status(503).json({ error: 'System info service not available' });
          return;
        }
        const systemInfo = systemInfoService.getSystemInfo();
        res.json({
          contents: [{
            uri: 'orchestrator://system/info',
            mimeType: 'application/json',
            text: JSON.stringify(systemInfo, null, 2),
          }],
        });
      } catch (error) {
        res.status(500).json({ error: String(error) });
      }
    });

    // Search memory
    this.app.get('/mcp/resources/orchestrator://memory/search', async (req, res) => {
      try {
        if (!this.dashboardServer) {
          res.status(503).json({ error: 'Dashboard server not available' });
          return;
        }
        const memoryService = (this.dashboardServer as any).memoryService;
        if (!memoryService) {
          res.status(503).json({ error: 'Memory service not available' });
          return;
        }
        const query = req.query.q as string || '';
        const limit = parseInt(req.query.limit as string || '10', 10);
        if (!query) {
          res.status(400).json({ error: 'Query parameter "q" is required' });
          return;
        }
        const results = await memoryService.search(query, { limit });
        res.json({
          contents: [{
            uri: `orchestrator://memory/search?q=${encodeURIComponent(query)}`,
            mimeType: 'application/json',
            text: JSON.stringify({ query, results, count: results.length }, null, 2),
          }],
        });
      } catch (error) {
        res.status(500).json({ error: String(error) });
      }
    });

    // Get Cursor project information
    this.app.get('/mcp/resources/orchestrator://cursor/project', async (req, res) => {
      try {
        if (!this.dashboardServer) {
          res.status(503).json({ error: 'Dashboard server not available' });
          return;
        }
        const cursorProjectService = (this.dashboardServer as any).cursorProjectService;
        if (!cursorProjectService) {
          res.status(503).json({ error: 'Cursor project service not available' });
          return;
        }
        const projectInfo = await cursorProjectService.getProjectStatus();
        res.json({
          contents: [{
            uri: 'orchestrator://cursor/project',
            mimeType: 'application/json',
            text: JSON.stringify(projectInfo, null, 2),
          }],
        });
      } catch (error) {
        res.status(500).json({ error: String(error) });
      }
    });

    // Health check endpoint
    this.app.get('/mcp/health', (req, res) => {
      res.json({ 
        status: 'healthy',
        port: this.port,
        timestamp: new Date().toISOString(),
      });
    });

    // MCP Tools
    this.app.get('/mcp/tools', (req, res) => {
      const tools: McpTool[] = [
        {
          name: 'get_workflow_status',
          description: 'Get the current status of a workflow',
          inputSchema: {
            type: 'object',
            properties: {
              workflowName: {
                type: 'string',
                description: 'Name of the workflow',
              },
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
              workflowName: {
                type: 'string',
                description: 'Name of the workflow',
              },
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
        {
          name: 'get_system_info',
          description: 'Get system information and resource metrics',
          inputSchema: {
            type: 'object',
            properties: {},
          },
        },
        {
          name: 'search_memory',
          description: 'Search memory entries',
          inputSchema: {
            type: 'object',
            properties: {
              query: {
                type: 'string',
                description: 'Search query',
              },
              limit: {
                type: 'number',
                description: 'Maximum number of results',
              },
            },
            required: ['query'],
          },
        },
        {
          name: 'get_cursor_project',
          description: 'Get Cursor project information',
          inputSchema: {
            type: 'object',
            properties: {},
          },
        },
      ];
      res.json({ tools });
    });

    // Execute MCP tool
    this.app.post('/mcp/tools/:toolName', async (req, res) => {
      const toolName = req.params.toolName;
      const args = req.body.arguments || {};

      try {
        if (!this.dashboardServer && toolName !== 'list_workflows') {
          res.status(503).json({ error: 'Dashboard server not available' });
          return;
        }
        
        let result: any;
        
        switch (toolName) {
          case 'get_workflow_status':
            if (!this.dashboardServer) {
              result = { error: 'Dashboard server not available' };
              break;
            }
            const metrics = await (this.dashboardServer as any).getMetrics();
            const workflow = metrics.workflows.find((w: any) => w.workflowName === args.workflowName);
            result = workflow || { error: 'Workflow not found' };
            break;

          case 'get_workflow_activity':
            const current = this.activityTracker.getCurrentActivity(args.workflowName);
            const recent = this.activityTracker.getRecentActivities(args.workflowName, 20);
            result = { current, recent };
            break;

          case 'list_active_workflows':
            if (!this.dashboardServer) {
              result = { workflows: [] };
              break;
            }
            const allMetrics = await (this.dashboardServer as any).getMetrics();
            result = {
              workflows: allMetrics.workflows.filter((w: any) => w.status === 'running'),
            };
            break;

          case 'get_system_info':
            if (!this.dashboardServer) {
              result = { error: 'Dashboard server not available' };
              break;
            }
            const systemInfoService = (this.dashboardServer as any).systemInfoService;
            if (!systemInfoService) {
              result = { error: 'System info service not available' };
              break;
            }
            result = systemInfoService.getSystemInfo();
            break;

          case 'search_memory':
            if (!this.dashboardServer) {
              result = { error: 'Dashboard server not available' };
              break;
            }
            const memoryService = (this.dashboardServer as any).memoryService;
            if (!memoryService) {
              result = { error: 'Memory service not available' };
              break;
            }
            const searchResults = await memoryService.search(args.query, { limit: args.limit || 10 });
            result = { results: searchResults, count: searchResults.length };
            break;

          case 'get_cursor_project':
            if (!this.dashboardServer) {
              result = { error: 'Dashboard server not available' };
              break;
            }
            const cursorProjectService = (this.dashboardServer as any).cursorProjectService;
            if (!cursorProjectService) {
              result = { error: 'Cursor project service not available' };
              break;
            }
            result = await cursorProjectService.getProjectStatus();
            break;

          default:
            res.status(404).json({ error: `Tool "${toolName}" not found` });
            return;
        }

        res.json({
          content: [
            {
              type: 'text',
              text: JSON.stringify(result, null, 2),
            },
          ],
        });
      } catch (error) {
        res.status(500).json({
          error: String(error),
          content: [
            {
              type: 'text',
              text: `Error: ${error}`,
            },
          ],
        });
      }
    });

    // Health check
    this.app.get('/health', (req, res) => {
      res.json({ status: 'ok', service: 'mcp-server' });
    });
  }

  async start(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.server.listen(this.port, () => {
        console.log(`🔌 MCP Server started on http://localhost:${this.port}`);
        console.log(`   Resources: http://localhost:${this.port}/mcp/resources`);
        console.log(`   Tools: http://localhost:${this.port}/mcp/tools`);
        resolve();
      });

      this.server.on('error', (error: any) => {
        if (error.code === 'EADDRINUSE') {
          console.error(`❌ Port ${this.port} is already in use. Try a different port with --port`);
          reject(error);
        } else {
          reject(error);
        }
      });
    });
  }

  async stop(): Promise<void> {
    return new Promise((resolve) => {
      this.server.close(() => {
        console.log('🔌 MCP Server stopped');
        resolve();
      });
    });
  }
}
