.PHONY: start stop status build-all build-docker build-dotnet build-rust install-node test clean help

help: ## Show this help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

start: ## Start all sovereign services
	bash scripts/levantar-todo.sh

stop: ## Stop all services
	docker compose -f docker-compose.sovereign.yml down

status: ## Show service status
	bash scripts/estado.sh

build-all: build-docker build-dotnet build-rust ## Build everything

build-docker: ## Build Docker containers
	docker compose -f docker-compose.sovereign.yml build

build-dotnet: ## Build .NET solutions
	@find 08-dotnet -name "*.sln" -maxdepth 4 | while read sln; do \
		echo "Building $$sln..."; \
		dotnet build "$$sln" --configuration Release || true; \
	done

build-rust: ## Build Rust components
	cd 12-rust/mamey-forge && cargo build --release 2>/dev/null || true
	cd 12-rust/mamey-node-rust && cargo build --release 2>/dev/null || true

install-node: ## Install Node.js dependencies
	@find 03-backend -name "package.json" -maxdepth 2 | while read pkg; do \
		dir=$$(dirname "$$pkg"); \
		echo "Installing $$dir..."; \
		cd "$$dir" && npm install && cd -; \
	done

test: ## Run all tests
	@echo "Running tests across all services..."
	@find 03-backend -name "package.json" -maxdepth 2 -exec sh -c 'cd $$(dirname {}) && npm test --if-present 2>/dev/null' \;

clean: ## Clean build artifacts
	find . -name "node_modules" -type d -prune -exec rm -rf {} + 2>/dev/null || true
	find . -name "bin" -type d -path "*/08-dotnet/*" -prune -exec rm -rf {} + 2>/dev/null || true
	find . -name "obj" -type d -path "*/08-dotnet/*" -prune -exec rm -rf {} + 2>/dev/null || true
	find . -name "target" -type d -path "*/12-rust/*" -prune -exec rm -rf {} + 2>/dev/null || true
