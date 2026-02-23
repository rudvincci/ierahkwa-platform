# Documentation Sync

This documentation is automatically synced from the library README files in `src/Mamey.*/README.md`.

## Ensuring Documentation Matches Source

To ensure all documentation in `docs/libraries/` matches the source library README files, run:

```bash
./sync-docs.sh
```

This script will:
1. Compare each library README in `src/Mamey.*/README.md` with its corresponding file in `docs/libraries/`
2. Copy any files that differ from source
3. Report synced and up-to-date files

## Important Notes

- **Never manually edit files in `docs/libraries/`** - These should always match the source library README files
- **Edit library READMEs in `src/Mamey.*/README.md`** - Then run `sync-docs.sh` to update docs
- Framework-level documentation (guides, getting-started, examples) can be edited directly in `docs/`

## Special Cases

- **Mamey.Persistence.MySQL** - Uses the more comprehensive README from `Mamey.Persistence.MySQL` (uppercase) rather than `Mamey.Persistence.MySql` (lowercase)

## Verification

To verify all files match:

```bash
# Check for mismatches
for lib_dir in src/Mamey.*; do
  lib=$(basename "$lib_dir")
  if [ -f "$lib_dir/README.md" ]; then
    lib_name=$(echo "$lib" | sed 's/^Mamey\.//' | tr '[:upper:]' '[:lower:]' | sed 's/\./-/g')
    for category in core cqrs messaging auth identity persistence observability infrastructure integration ui utilities standards azure http docs; do
      if [ -f "docs/libraries/$category/$lib_name.md" ]; then
        if ! diff -q "$lib_dir/README.md" "docs/libraries/$category/$lib_name.md" > /dev/null 2>&1; then
          echo "✗ $lib differs"
        fi
        break
      fi
    done
  fi
done
```






