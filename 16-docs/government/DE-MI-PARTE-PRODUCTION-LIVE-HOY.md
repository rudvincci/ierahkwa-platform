# De mi parte: dónde ir para comprobar y salir a production live hoy

Pasos concretos (terminal + navegador) para **comprobar** que todo está bien y **salir a producción en vivo** hoy.

---

## Dónde estás trabajando

- **Raíz del proyecto:** la carpeta donde están `start.sh`, `status.sh` y la carpeta `RuddieSolution/`.
- **Terminal:** abrirla en esa raíz (o ir con `cd` hasta ahí).

---

## 1. Comprobar (terminal)

En la terminal, **desde la raíz del repo**:

```bash
./scripts/prepare-ready.sh
```

- Si sale **READY** → .env y archivos de datos OK. Sigue al paso 2.
- Si sale **NOT READY** → corrige lo que indique (sobre todo `.env` con JWT de 32+ caracteres y que existan los archivos en `RuddieSolution/node/data/`). Vuelve a ejecutar hasta ver READY.

*(Si el Node aún no está arrancado, puedes usar antes `SKIP_SERVER=1 ./scripts/prepare-ready.sh` para comprobar solo .env y datos.)*

---

## 2. Arrancar (terminal)

```bash
./start.sh
```

Espera a que levante Node (8545) y Banking Bridge (3001). Si ya estaban corriendo, no hace falta volver a arrancar.

---

## 3. Comprobar estado (terminal)

```bash
./status.sh
```

Revisa que veas:

- Node (8545) **ONLINE**
- Banking Bridge **ONLINE**
- **Banco (BHBK) OK** (registro + bridge conectado)

Si algo sale OFFLINE o en rojo, revisa logs o que no haya otro proceso usando los puertos 8545 y 3001.

---

## 4. Verificación 100% production (terminal)

```bash
cd RuddieSolution/node && node scripts/verificar-production-live.js
```

- **Exit 0** → listo para production live.
- **Exit 1** → corrige lo que falle (falta algún archivo de datos o algún endpoint no responde).

---

## 5. Dónde entrar en el navegador (production live hoy)

Con el Node en **127.0.0.1:8545** (o tu IP/host si ya apuntas un dominio):

| Qué comprobar | URL en el navegador |
|---------------|----------------------|
| **Plataforma / Admin** | http://127.0.0.1:8545/platform/admin.html |
| **Health del Node** | http://127.0.0.1:8545/health |
| **BDET Bank (back único)** | http://127.0.0.1:8545/platform/bdet-bank.html o http://127.0.0.1:8545/bdet-bank |
| **Estado del banco (API)** | http://127.0.0.1:8545/api/v1/bdet/bank-status |
| **Operatividad 24/7** | http://127.0.0.1:8545/api/operativity |

Si usas **otro host o dominio** (ej. `https://app.ierahkwa.gov`), sustituye `http://127.0.0.1:8545` por tu URL base.

---

## 6. Salir “en vivo” de verdad (con dominio e HTTPS)

Para que sea **production live** con dominio público y HTTPS:

1. **Dominio:** que tu servidor tenga IP pública y que el dominio (ej. `ierahkwa.gov`) apunte a esa IP (registro A o CNAME).
2. **Certificados:** en el servidor, ejecutar  
   `sudo ./scripts/setup-ssl-certbot-nginx.sh`  
   (o seguir `docs/CERTIFICADOS-SSL-TLS.md`).
3. **Nginx:** tener nginx (o Caddy) como reverse proxy con TLS; config de referencia: `DEPLOY-SERVERS/nginx-load-balancer.conf`.
4. **Acceso final:** entrar por `https://tu-dominio/...` (ej. `https://app.ierahkwa.gov/platform/admin.html`).

Mientras tanto, **“live” en tu máquina** = arrancar con `./start.sh`, comprobar con `./status.sh` y abrir las URLs del paso 5 en el navegador.

---

## Resumen rápido (orden)

1. **Terminal:** `./scripts/prepare-ready.sh` → READY  
2. **Terminal:** `./start.sh`  
3. **Terminal:** `./status.sh` → todo ONLINE y Banco OK  
4. **Terminal:** `cd RuddieSolution/node && node scripts/verificar-production-live.js` → exit 0  
5. **Navegador:** abrir http://127.0.0.1:8545/platform/admin.html y http://127.0.0.1:8545/platform/bdet-bank.html para comprobar  
6. **Production con dominio:** configurar DNS + certificados + nginx y usar tus URLs HTTPS

Con eso **compruebas** todo y **sales a production live** (hoy en local; con dominio cuando tengas DNS + SSL).

---

*Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister.*
