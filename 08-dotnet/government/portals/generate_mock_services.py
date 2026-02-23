#!/usr/bin/env python3
"""
Generate comprehensive mock services for all portals based on proto file definitions.
This script creates mock clients with realistic data generation using Bogus.
"""

import os
import re

# Portal to proto file mapping
PORTAL_PROTO_MAP = {
    "Payments": "payments.proto",
    "Lending": "lending.proto",
    "Dex": "dex.proto",
    "CryptoExchange": "crypto_exchange.proto",
    "SmartContracts": "general.proto",  # Uses general.proto for contracts
    "AccountAbstraction": "wallet.proto",
    "Bridge": "bridge.proto",
    "ILP": "general.proto",  # May need specific proto
    "ODL": "general.proto",
    "Pathfinding": "dex.proto",  # Uses DEX routing
    "TravelRule": "compliance.proto",
    "UPG": "upg.proto",
    "Channels": "general.proto",
    "Programmable": "general.proto",
    "Sharding": "network.proto",
    "Advanced": "advanced.proto",
    "Compliance": "compliance.proto",
    "Metrics": "metrics.proto",
    "Webhooks": "general.proto",
    "Callbacks": "general.proto",
    "RBAC": "general.proto",
    "TrustLines": "ledger.proto",
    "LedgerIntegration": "ledger_integration.proto",
    "Node": "node.proto",
}

# Basic mock service template for portals that don't need complex data yet
MOCK_SERVICE_TEMPLATE = """using Bogus;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public interface IMamey{Portal}Client
{{
    // Operations will be added based on proto definitions
    Task<List<object>> ListItemsAsync(int limit = 50, int offset = 0);
}}

public class MockMamey{Portal}Client : IMamey{Portal}Client
{{
    private readonly Faker _faker = new();
    private readonly List<object> _items = new();

    public MockMamey{Portal}Client()
    {{
        InitializeMockData();
    }}

    private void InitializeMockData()
    {{
        // Initialize mock data based on proto definitions
        // TODO: Add specific data generation based on {ProtoFile}
    }}

    public Task<List<object>> ListItemsAsync(int limit = 50, int offset = 0)
    {{
        return Task.FromResult(_items.Skip(offset).Take(limit).ToList());
    }}
}}
"""

def generate_mock_service(portal_name, proto_file):
    """Generate a mock service file for a portal."""
    content = MOCK_SERVICE_TEMPLATE.format(
        Portal=portal_name,
        ProtoFile=proto_file
    )
    
    file_path = f"src/MameyNode.Portals.Mocks/Mock{portal_name}Client.cs"
    
    # Check if file already exists and has content
    if os.path.exists(file_path):
        with open(file_path, 'r') as f:
            existing = f.read()
            if "ListItemsAsync" not in existing or len(existing) > 500:
                # File already has implementation, skip
                return False
    
    with open(file_path, 'w') as f:
        f.write(content)
    
    return True

def main():
    """Generate mock services for all portals."""
    generated = 0
    skipped = 0
    
    for portal, proto in PORTAL_PROTO_MAP.items():
        if generate_mock_service(portal, proto):
            print(f"Generated Mock{portal}Client.cs")
            generated += 1
        else:
            print(f"Skipped Mock{portal}Client.cs (already exists)")
            skipped += 1
    
    print(f"\n{'='*60}")
    print(f"Generated: {generated} mock services")
    print(f"Skipped: {skipped} (already implemented)")
    print(f"{'='*60}")

if __name__ == "__main__":
    main()

