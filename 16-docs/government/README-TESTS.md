# 🧪 Guía de Tests - IERAHKWA Platform

## Ejecutar Todos los Tests

### Node.js (Jest)
```bash
cd node
npm install
npm test
```

**Tests incluidos**:
- `tests/kms.test.js` - Key Management Service
- `tests/quantum.test.js` - Quantum Encryption
- `tests/proxies.test.js` - Multilang Proxies
- `tests/auth.test.js` - Authentication

**Coverage**: `npm test -- --coverage`

---

### Python (pytest)
```bash
cd services/python
pip install -r requirements.txt
pytest -v
```

**Tests incluidos**:
- `tests/test_fraud.py` - Fraud Detection
- `tests/test_risk.py` - Risk Scoring

**Coverage**: `pytest --cov=. --cov-report=html`

---

### Rust (cargo test)
```bash
cd services/rust
cargo test
```

**Tests incluidos**:
- `src/tests.rs` - SWIFT MT/MX parsing

**Con verbose**: `cargo test -- --nocapture`

---

### Go (go test)
```bash
cd services/go
go test -v
```

**Tests incluidos**:
- `main_test.go` - Queue operations

**Con race detection**: `go test -race -v`

**Coverage**: `go test -coverprofile=coverage.out && go tool cover -html=coverage.out`

---

## CI/CD

Los tests se ejecutan automáticamente en GitHub Actions:
- Push a `main` o `develop`
- Pull Requests

Ver: `.github/workflows/ci.yml`
