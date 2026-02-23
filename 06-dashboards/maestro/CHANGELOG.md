# Changelog

All notable changes to Maestro will be documented in this file.

## [Unreleased]

### Added
- **Custom Dialog System**: Replaced browser alerts with beautiful in-app modal dialogs
- **Immediate Workflow Display**: Workflows now appear instantly when started with "spinning up" status
- **Smart Stop Button**: Automatically enables/disables based on active workflow count
- **Responsive Dashboard Design**: Dashboard adapts to mobile, tablet, and desktop screen sizes
- **System Information Panel**: Real-time monitoring of Maestro process (PID, RAM, CPU usage, uptime)
- **Heap Memory Configuration**: Configurable Node.js heap memory size (default: 8GB) to prevent crashes
- **Enhanced MCP Server**: Improved startup reliability and error handling
- **Improved `maestro disable`**: Better process detection using PID file and port-based fallback

### Changed
- **Dashboard UI**: Simplified workflow creation with prompt-based generation
- **Workflow Display**: Enhanced workflow cards with better metrics and responsive layout
- **MCP Server**: Starts automatically with dashboard by default
- **Process Management**: Improved PID detection and cleanup in `maestro disable` command
- **Error Handling**: Better error messages and user feedback throughout the dashboard

### Fixed
- **Workflow Visibility**: Fixed issue where workflows didn't appear immediately after starting
- **Stop Button State**: Stop button now correctly enables/disables based on active workflows
- **MCP Server Startup**: Fixed MCP server not starting automatically on dashboard startup
- **Process Detection**: Fixed `maestro disable` not finding processes when PID file missing
- **Memory Issues**: Added configurable heap memory to prevent crashes during large workflows
- **Dashboard Updates**: Fixed real-time updates for workflow duration, success rate, and current step
- **JavaScript Compatibility**: Fixed ES6+ syntax issues for better browser compatibility

### Technical Improvements
- **Heap Memory**: All Node.js processes now use configurable heap memory (default: 8GB)
- **PID Management**: Enhanced process detection using both PID files and port scanning
- **WebSocket Updates**: Improved real-time update reliability
- **Responsive CSS**: Added media queries for mobile, tablet, and desktop layouts
- **Error Recovery**: Better handling of stale PID files and dead processes

## Configuration Changes

### New Configuration Option

Add to `.maestro/config/orchestrator.config.yml`:

```yaml
execution:
  # Node.js heap memory size in MB (default: 8192 = 8GB)
  heapMemorySize: 8192  # Increase if experiencing crashes
```

Or set environment variable:
```bash
export MAESTRO_HEAP_SIZE=16384  # 16GB
```

## Migration Notes

- **No breaking changes** - All updates are backward compatible
- **Default heap memory** is now 8GB (was Node.js default ~1.5GB)
- **MCP server** now starts automatically with dashboard
- **Custom dialogs** replace browser alerts (no user action required)
