## Multi-Language Architecture

Single system, four languages (C#, Rust, TypeScript, PHP).
The C# project is the source of truth for shared specs.

Layered design:
1) Domain: Accounts, Transactions, Wallets, Government, Compliance, VIP, Interpol
2) API: REST/JSON endpoints defined in SYNC_OPENAPI.yaml
3) Storage: Each language has its own DB adapter
4) Node: Connects to MameyNode via RPC/bridge

Sync strategy:
- Specs are authored in C# docs (this folder)
- Scripts copy specs to all projects
- Implementations must match the shared specs

Special modules:
- Interpol (SLTD, notices, ePassport, audit) must be implemented in all languages
- Insurance/Customs/Logistics are tracked here even if not yet implemented in code

Implementation targets:
- Rust: /Users/ruddie/Desktop/software/ruddie-mameynode
- PHP: /Users/ruddie/Desktop/software/mameynode-php
- TypeScript BackOffice: /Users/ruddie/Downloads/Inkg-BackOffice-main
- TypeScript FrontEnd: /Users/ruddie/Downloads/Inkg-FrontEnd-main
