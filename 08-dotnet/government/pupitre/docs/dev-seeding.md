# Development Seed Data

To populate a local Pupitre environment with recognizable demo data, use the `Pupitre.DevSeeder` console that now lives under `tools/`.

## Prerequisites

1. Run the full Docker Compose stack so that the API Gateway (port 60000) and all Foundation services are reachable.
2. Ensure RabbitMQ, PostgreSQL, MongoDB, and Redis containers are healthy (they are required by the target services).

## Running the Seeder

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Pupitre
# Restore/build once
dotnet build Pupitre.sln

# Seed data through the API gateway (default http://localhost:60000/api/)
dotnet run --project tools/Pupitre.DevSeeder/Pupitre.DevSeeder.csproj
```

### Custom options

- `--api-base=<url>` – Override the API base URL (default `http://localhost:60000/api/`).
- `--dry-run` (or environment variable `PUPITRE_SEED_DRYRUN=1`) – Print payloads without issuing HTTP calls.
- Environment variable `PUPITRE_API_BASE` is also honored.

## What gets created?

The seeder posts the canonical `Add*` commands through the API Gateway:

| Endpoint | Sample Records |
|----------|----------------|
| `POST /api/users` | Two students (STUDENT-001/002) with metadata and profiles |
| `POST /api/gles` | Math & Literacy GLEs tied to the sample students |
| `POST /api/lessons` | Two STEAM lesson plans authored by mock educators |
| `POST /api/assessments` | Benchmark + rubric artifacts used for analytics |

All payloads include deterministic GUIDs, so re-running the tool is idempotent. Conflicts or validation failures are logged rather than treated as fatal errors.

## Extending the dataset

1. Add new entries to `SampleData` in `tools/Pupitre.DevSeeder/Program.cs`.
2. Rebuild the solution.
3. Rerun the seeder.

Feel free to expand the tool to hit additional services or to read JSON fixtures instead of the in-code samples once more detailed domain requirements are available.
