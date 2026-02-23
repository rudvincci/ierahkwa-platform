# Hello World

The simplest possible MameyFutureNode smart contract — stores and retrieves a greeting.

## Functions

- `init()` — Sets greeting to "Hello, MameyFutureNode!"
- `execute_set_greeting(message)` — Update the greeting
- `query_greeting()` — Read the current greeting

## Build & Run

```bash
mameyforge build
mameyforge test
mameyforge devnet start
mameyforge deploy --network devnet
mameyforge query --contract <ADDR> --function query_greeting
```
