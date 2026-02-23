# Templates

MameyForge ships with built-in templates to scaffold common contract types quickly. You can also create your own.

## Built-in Templates

### `basic` (default)

Minimal contract with `init`, `execute`, and `query` entry points.

```bash
mameyforge init my-project
# or explicitly:
mameyforge init my-project --template basic
```

**What you get:**
- Owner-gated key-value storage
- `init()` — Initialise with owner
- `execute_set(key, value)` — Store a value (owner only)
- `execute_delete(key)` — Delete a value (owner only)
- `query_get(key)` — Retrieve a value
- `query_owner()` — Get owner address

### `token`

ERC-20-like fungible token.

```bash
mameyforge init my-token --template token
```

**What you get:**
- `init(name, symbol, decimals, initial_supply)` — Set up the token
- `transfer(to, amount)` — Direct transfer
- `transfer_from(from, to, amount)` — Delegated transfer
- `approve(spender, amount)` — Allowance approval
- `mint(to, amount)` — Mint tokens (minter role)
- `burn(amount)` — Burn tokens
- `pause()` / `unpause()` — Emergency stop
- Queries: `name()`, `symbol()`, `decimals()`, `total_supply()`, `balance_of(account)`, `allowance(owner, spender)`

### `nft`

ERC-721-like non-fungible token.

```bash
mameyforge init my-nft --template nft
```

**What you get:**
- `init(name, symbol)` — Create the collection
- `mint(to, uri)` — Auto-incrementing mint
- `burn(token_id)` — Burn a token
- `transfer(from, to, token_id)` — Transfer with authorisation checks
- `approve(approved, token_id)` — Per-token approval
- `set_approval_for_all(operator, approved)` — Operator approval
- `set_base_uri(uri)` / `set_token_uri(token_id, uri)` — Metadata
- `pause()` / `unpause()` — Emergency stop
- Queries: `name()`, `symbol()`, `total_supply()`, `balance_of(owner)`, `owner_of(token_id)`, `token_uri(token_id)`, `get_approved(token_id)`, `is_approved_for_all(owner, operator)`

### `governance`

DAO governance with proposals and voting (inline template).

```bash
mameyforge init my-dao --template governance
```

### `multi-token`

ERC-1155-like multi-token standard (inline template).

```bash
mameyforge init my-collection --template multi-token
```

## Template Files

File-based templates live in `MameyForge/templates/<name>/`:

```
templates/
├── basic/
│   ├── Cargo.toml.template
│   ├── mameyforge.toml.template
│   ├── README.md.template
│   └── src/
│       └── lib.rs.template
├── token/
│   ├── Cargo.toml.template
│   ├── mameyforge.toml.template
│   ├── README.md.template
│   └── src/
│       └── lib.rs.template
└── nft/
    ├── Cargo.toml.template
    ├── mameyforge.toml.template
    ├── README.md.template
    └── src/
        └── lib.rs.template
```

## Placeholders

Templates use `{{placeholder}}` syntax. Supported placeholders:

| Placeholder | Replaced With |
|-------------|---------------|
| `{{project_name}}` | Project name from `mameyforge init <name>` |
| `{{version}}` | `0.1.0` (default) |
| `{{author}}` | Git user or empty string |

## Creating Custom Templates

1. **Create a directory** under `templates/`:

   ```bash
   mkdir -p templates/my-template/src
   ```

2. **Add template files** with `.template` extension:

   ```
   templates/my-template/
   ├── Cargo.toml.template
   ├── mameyforge.toml.template
   ├── README.md.template
   └── src/
       └── lib.rs.template
   ```

3. **Use placeholders** in the files:

   ```rust
   //! {{project_name}} — My Custom Contract
   ```

4. **Use it:**

   ```bash
   mameyforge init my-project --template my-template
   ```

The `init` command looks for a matching directory in `templates/`. If found, it reads `.template` files, replaces placeholders, and writes the output. If no file-based template is found, the built-in inline templates are used as a fallback.

## Tips

- Keep templates minimal — they're starting points, not finished products
- Use `mamey-contracts-shared` for host function bindings and storage helpers
- Use `mamey-contracts-base` for security patterns and typed errors
- All contracts compile to WASM via `wasm32-unknown-unknown`
- Templates should include basic tests in a `#[cfg(test)]` module
