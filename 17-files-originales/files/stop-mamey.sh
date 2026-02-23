#!/bin/bash
# 🛑 STOP MAMEY — Graceful shutdown
set -euo pipefail
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
LOGS="$DIR/logs"

echo "🛑 Stopping Mamey services..."

for pidfile in "$LOGS"/*.pid; do
    [ -f "$pidfile" ] || continue
    pid=$(cat "$pidfile")
    name=$(basename "$pidfile" .pid)
    if kill -0 "$pid" 2>/dev/null; then
        kill "$pid" 2>/dev/null
        echo "  ✓ $name (PID $pid) stopped"
    fi
    rm -f "$pidfile"
done

# Kill orphans
for port in 8545 5001 5002 5003; do
    lsof -ti ":$port" 2>/dev/null | xargs kill 2>/dev/null || true
done

echo "✅ All services stopped"
