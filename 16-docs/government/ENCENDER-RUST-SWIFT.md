# Encender Rust SWIFT Service (puerto 8590)

**Sovereign Government of Ierahkwa Ne Kanienke**

El servicio **Rust SWIFT** (puerto 8590) aparece como **apagado** si Rust no está configurado o el binario no está compilado. Pasos para dejarlo encendido.

---

## 1. Instalar / configurar Rust (una vez)

Si ya tienes `rustup` pero cargo falla con *"no default toolchain"*:

```bash
rustup default stable
```

Si no tienes Rust:

```bash
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source "$HOME/.cargo/env"
rustup default stable
```

---

## 2. Compilar el servicio

Desde la raíz del repo:

```bash
cd RuddieSolution/services/rust
cargo build --release
```

(O desde la raíz: `cd services/rust` si usas la carpeta `services` de la raíz.)

El binario quedará en: `target/release/ierahkwa-swift-service`.

---

## 3. Arrancar todo de nuevo

Desde la raíz:

```bash
./start.sh
```

El script detecta `target/release/ierahkwa-swift-service` y arranca el servicio Rust en el puerto 8590.

---

## 4. Comprobar

```bash
curl -s http://localhost:8590/health
```

O en el **Dashboard Tests Live**: Rust SWIFT Service debería aparecer en verde (healthy).

---

## Resumen

| Paso | Comando |
|------|---------|
| Configurar Rust | `rustup default stable` |
| Compilar | `cd RuddieSolution/services/rust && cargo build --release` |
| Encender todo | `./start.sh` |
| Verificar | `curl http://localhost:8590/health` |

Si no necesitas SWIFT/MT-MX en este entorno, puedes dejar el servicio apagado; el resto de la plataforma funciona sin él.
