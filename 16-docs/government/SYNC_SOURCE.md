## Multi-Language Sync Source of Truth

This file defines the synchronization contract for the MameyNode platform.
All language implementations (C#, Rust, TypeScript, PHP) MUST match the specs.

Source of truth:
- C# docs: docs/SYNC_SOURCE.md
- Specs: docs/SYNC_OPENAPI.yaml, docs/SYNC_MODELS.json, docs/SYNC_ARCHITECTURE.md

Workflow:
1) Update C# domain code
2) Update specs in docs/ (these 4 files)
3) Run scripts/sync-mamey-multi.sh
4) Implement matching logic in Rust/TypeScript/PHP
5) Run tests in each language

Rules:
- Do not edit spec files in target repos
- Only update specs in the C# docs folder
- Keep endpoints and models identical across languages

Targets:
- Rust: /Users/ruddie/Desktop/software/ruddie-mameynode
- PHP: /Users/ruddie/Desktop/software/mameynode-php
- TypeScript BackOffice: /Users/ruddie/Downloads/Inkg-BackOffice-main
- TypeScript FrontEnd: /Users/ruddie/Downloads/Inkg-FrontEnd-main
