```
███╗   ███╗ █████╗ ███╗   ███╗███████╗███████╗████████╗██████╗  ██████╗ 
████╗ ████║██╔══██╗████╗ ████║██╔════╝██╔════╝╚══██╔══╝██╔══██╗██╔═══██╗
██╔████╔██║███████║██╔████╔██║█████╗  ███████╗   ██║   ██████╔╝██║   ██║
██║╚██╔╝██║██╔══██║██║╚██╔╝██║██╔══╝  ╚════██║   ██║   ██╔══██╗██║   ██║
██║ ╚═╝ ██║██║  ██║██║ ╚═╝ ██║███████╗███████║   ██║   ██║  ██║╚██████╔╝
╚═╝     ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝╚══════╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝ 
                                                                        
     ██████╗ ██████╗  ██████╗██╗  ██╗███████╗██████╗ ███████╗████████╗
    ██╔════╝██╔═══██╗██╔════╝██║  ██║██╔════╝██╔══██╗██╔════╝╚══██╔══╝
    ██║     ██║   ██║██║     ███████║█████╗  ██████╔╝███████╗   ██║   
    ██║     ██║   ██║██║     ██╔══██║██╔══╝  ██╔══██╗╚════██║   ██║   
    ╚██████╗╚██████╔╝╚██████╗██║  ██║███████╗██║  ██║███████║   ██║   
     ╚═════╝ ╚═════╝  ╚═════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚══════╝   ╚═╝   
```

# Maestro 🎼

> **Production-Grade AI Agent Orchestration Platform**  
> Coordinate multiple AI agents through workflow definitions. Execute complex tasks across different agent roles with dependency management, parallel execution, and intelligent monitoring.

**🎯 Maestro addresses current AI development limitations** by providing multi-agent orchestration, intelligent context management, persistent memory integration, and comprehensive monitoring. **📚 Learn how to train your AI models** with TDDs, implementation plans, and best practices in our [AI Training Guide](docs/guides/AI_TRAINING_AND_LIMITATIONS.md).

[![License: AGPL-3.0](https://img.shields.io/badge/License-AGPL--3.0-blue.svg)](https://opensource.org/licenses/AGPL-3.0)
[![Node.js Version](https://img.shields.io/badge/node-%3E%3D20.0.0-brightgreen.svg)](https://nodejs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.7-blue.svg)](https://www.typescriptlang.org/)

---

## 🚀 Quick Start

### Install & Run (Local)

```bash
# Install dependencies
cd .maestro
npm install
npm run build

# List available workflows
maestro flows

# Run a workflow
maestro run --flow my-workflow --runner cursor

# Start dashboard
maestro enable
# Access at http://localhost:3000
```

### Install & Run (Docker)

```bash
# Build and start
cd .maestro
npm run docker:build
npm run docker:run

# Execute workflows
npm run docker:exec flows
npm run docker:exec run --flow my-workflow
```

**📖 Need more details?** See [Installation Guide](docs/INSTALLATION.md) or [Quick Start Guide](docs/QUICK_START.md)

---

## ✨ Key Features

### 🎯 Core Capabilities

- **📋 Workflow Definitions** - Define complex multi-step workflows with YAML
- **👥 Agent Roles** - 13 pre-configured roles (Architect, Backend, Frontend, Tests, etc.)
- **🔗 Dependency Resolution** - Automatic topological sorting of workflow steps
- **⚡ Parallel Execution** - Run independent tasks simultaneously
- **🔄 Nested Workflows** - Execute workflows within workflows
- **🌱 Dynamic Task Spawning** - Create sub-tasks conditionally
- **🎨 Cursor CLI Integration** - Seamless integration with Cursor CLI

### 📊 Advanced Features

- **📈 Real-Time Dashboard** - Monitor workflows with live updates and responsive design
- **🎨 Custom Dialog System** - Beautiful in-app dialogs replace browser alerts
- **⚡ Immediate Workflow Display** - Workflows appear instantly with "spinning up" status
- **🎛️ Smart Controls** - Stop button automatically enables/disables based on active workflows
- **📱 Responsive UI** - Dashboard adapts to mobile, tablet, and desktop screens
- **🧠 Intelligent Monitoring** - AI-powered output analysis and re-alignment
- **💾 Checkpoint/Resume** - Save state and resume interrupted workflows
- **🎯 Model Selection** - Choose AI models per workflow or step
- **🔄 Agent Switching** - Switch agents mid-workflow dynamically
- **📊 Token Tracking** - Monitor token usage and context window utilization
- **🔍 Progress Tracking** - Track detailed task progress and sub-tasks
- **💾 Result Caching** - Cache results to avoid re-execution
- **💻 System Monitoring** - Real-time process info (PID, RAM, CPU usage)
- **⚙️ Configurable Memory** - Adjustable heap memory size (default: 8GB) to prevent crashes
- **🐳 Docker Support** - Run in containers for production deployments

---

## 📚 Documentation

### 🎓 Getting Started

| Document | Description |
|----------|-------------|
| [**Installation Guide**](docs/INSTALLATION.md) | Complete installation instructions for local and Docker |
| [**Quick Start**](docs/QUICK_START.md) | Get up and running in 5 minutes |
| [**Installation Options**](docs/INSTALLATION_OPTIONS.md) | Detailed comparison: Local vs Docker |
| [**Documentation Index**](docs/INDEX.md) | Complete documentation navigation hub |

### 📖 User Guides

| Document | Description |
|----------|-------------|
| [**Usage Guide**](docs/USAGE_GUIDE.md) | Comprehensive usage documentation |
| [**Workflow Guide**](docs/WORKFLOWS.md) | Creating and managing workflows |
| [**Dashboard Guide**](docs/DASHBOARD.md) | Using the real-time monitoring dashboard |
| [**CLI Reference**](docs/CLI_REFERENCE.md) | Complete CLI command reference |

### 🔧 Configuration

| Document | Description |
|----------|-------------|
| [**Configuration Guide**](docs/CONFIGURATION.md) | Configuration files and options (including heap memory) |
| [**Config Setup**](docs/CONFIG_SETUP.md) | Setting up orchestrator.config.yml |
| [**Private Data Sync**](docs/PRIVATE_SYNC.md) | Optional private repository sync |
| [**Privacy & Security**](PRIVACY.md) | Privacy guarantees and data handling |
| [**Changelog**](CHANGELOG.md) | Recent updates and improvements |

### 🐳 Deployment

| Document | Description |
|----------|-------------|
| [**Docker Guide**](docs/DOCKER.md) | Complete Docker setup and usage |
| [**Quick Start Docker**](docs/QUICK_START_DOCKER.md) | Fast Docker setup guide |

### 🔌 Integration

| Document | Description |
|----------|-------------|
| [**Cursor CLI Integration**](docs/CURSOR_AGENT_INTEGRATION.md) | Integrating with Cursor CLI |
| [**MCP Server**](MCP_SERVER_PROPOSAL.md) | Model Context Protocol server proposal |

### 🎯 Advanced Topics

| Document | Description |
|----------|-------------|
| [**Subagent Capabilities**](docs/SUBAGENT_CAPABILITIES.md) | Advanced workflow patterns |
| [**Enhanced Dashboard**](docs/guides/ENHANCED_DASHBOARD_FEATURES.md) | Dashboard features and capabilities |
| [**Workflow Creation**](docs/guides/WORKFLOW_CREATION_AND_MODEL_SELECTION.md) | Creating workflows and selecting models |
| [**AI Training & Limitations**](docs/guides/AI_TRAINING_AND_LIMITATIONS.md) | How Maestro addresses AI limitations and training best practices |

### 🛠️ Development

| Document | Description |
|----------|-------------|
| [**Contributing**](CONTRIBUTING.md) | Contributing guidelines |
| [**Development Setup**](docs/DEVELOPMENT.md) | Setting up development environment |
| [**Testing**](docs/TESTING.md) | Running and writing tests |
| [**Changelog**](CHANGELOG.md) | Version history and changes |

### 📋 Reference

| Document | Description |
|----------|-------------|
| [**Troubleshooting**](docs/reference/DASHBOARD_TROUBLESHOOTING.md) | Common issues and solutions |
| [**Changelog**](CHANGELOG_SUBAGENTS.md) | Version history and changes |

---

## 🎯 Use Cases

### 🏗️ Software Development

- **Feature Implementation** - Coordinate multiple agents to implement complete features
- **Code Reviews** - Automated code review workflows
- **Documentation** - Generate technical documentation
- **Testing** - Create and run test suites

### 🔍 Compliance & Analysis

- **TDD Compliance** - Verify services match TDD specifications
- **Architecture Analysis** - Analyze and validate architecture
- **Code Quality** - Run quality checks and validations

### 🚀 CI/CD Integration

- **Automated Workflows** - Run workflows in CI/CD pipelines
- **Quality Gates** - Enforce quality standards
- **Deployment Validation** - Validate deployments

### 🎓 AI Training & Quality Improvement

- **Context-Aware Development** - AI agents use TDDs, plans, and documentation for better results
- **Pattern Learning** - Maestro learns from your codebase patterns and conventions
- **Quality Assurance** - Automated checks ensure implementations match specifications
- **Best Practices** - Enforces coding standards and architectural patterns

**📖 Learn more:** [AI Training & Limitations Guide](docs/guides/AI_TRAINING_AND_LIMITATIONS.md)

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      Maestro CLI                             │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │   Workflow   │  │   Agent      │  │   Dashboard │     │
│  │   Executor   │  │   Runner     │  │   Server    │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────────────────────────┘
         │                    │                    │
         ▼                    ▼                    ▼
┌─────────────────────────────────────────────────────────────┐
│                    Cursor CLI                                │
│              (cursor-agent)                                 │
└─────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────┐
│                    AI Models                                 │
│  Claude 3.5 Sonnet | Claude 3 Opus | Claude 3 Haiku        │
└─────────────────────────────────────────────────────────────┘
```

---

## 📦 Installation Options

Maestro can run in two ways:

### 🖥️ Local Installation

**Best for:** Development, quick iterations, direct file access

```bash
npm install && npm run build
maestro flows
```

**Advantages:**
- ✅ Fastest performance
- ✅ Direct file access
- ✅ Easy debugging
- ✅ No Docker required

### 🐳 Docker Container

**Best for:** Production, CI/CD, isolated environments

```bash
npm run docker:build
npm run docker:run
```

**Advantages:**
- ✅ Complete isolation
- ✅ Consistent environment
- ✅ Production ready
- ✅ Easy deployment

**📖 See [Installation Options](docs/INSTALLATION_OPTIONS.md) for detailed comparison**

---

## 🔒 Privacy & Security

Maestro is **privacy-first**:

- ✅ **No Telemetry** - Zero data collection
- ✅ **Local Storage** - All data stored locally
- ✅ **Git Ignored** - User data never committed
- ✅ **Optional Private Sync** - Sync to private repo (opt-in)

**📖 See [Privacy Guide](docs/PRIVACY.md) for details**

---

## 🛠️ Requirements

- **Node.js** 20+ (for local installation)
- **Docker** 20.10+ (for container installation)
- **Cursor CLI** (optional, for full functionality)

---

## 📄 License

**AGPL-3.0** - See [LICENSE](LICENSE) file for details

---

## 🤝 Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

## 📞 Support

- **Documentation**: See [docs/](docs/) directory
- **Issues**: [GitHub Issues](https://github.com/Mamey-io/Maestro/issues)
- **Discussions**: [GitHub Discussions](https://github.com/Mamey-io/Maestro/discussions)

---

## 🌟 Star History

If you find Maestro useful, please consider giving it a ⭐ on GitHub!

---

<div align="center">

**Made with ❤️ by [Mamey Technologies](https://mamey.io)**

[Website](https://mamey.io) • [Documentation](docs/) • [GitHub](https://github.com/Mamey-io/Maestro)

</div>
