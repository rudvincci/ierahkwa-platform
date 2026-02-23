# Recomendaciones Aplicadas

**Fecha:** 2 Feb 2026

## 1. Node.js 18+

- **Hecho:** Instalado Node.js v18.20.8 via nvm
- **Script:** `start.sh` ahora carga nvm y usa `nvm use 18` antes de arrancar servicios
- **Comando manual:** `nvm use 18` (o `nvm use 20`) en tu terminal para usar Node 18

## 2. Rust/Cargo

- **Hecho:** Instalado Rust 1.93.0 via rustup
- **Ruta:** `~/.cargo/bin` (agregar a PATH si no se detecta)
- **Servicio:** Rust SWIFT (8590) debería arrancar si existe `services/rust/` con `cargo run`

## 3. Python FastAPI

- **Hecho:** Script `scripts/install-python-ml.sh` actualizado para usar venv
- **Pasos:**
  ```bash
  ./scripts/install-python-ml.sh
  ```
- **Ubicación:** `RuddieSolution/services/python/.venv`
- **Servicio:** `start-all.sh` usa `./run.sh` (venv + pip install + uvicorn)

## 4. Lista de nodos (Plataforma Vigilancia)

- **API:** `GET /api/v1/security/nodes` existe en `server.js`
- **Página:** `plataforma-vigilancia-inteligencia.html` → pestaña "Nodos"
- Si ves "No se pudo cargar la lista de nodos": verifica que el servidor Node (8545) esté activo y que la pestaña "Nodos" se haya abierto (la carga es bajo demanda)

## Comandos útiles

```bash
# Usar Node 18
nvm use 18

# Instalar deps Python ML (venv)
./scripts/install-python-ml.sh

# Arrancar todo
./start.sh

# Ver estado
./status.sh
```
