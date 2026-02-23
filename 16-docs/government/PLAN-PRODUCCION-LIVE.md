# Plan Producción Live

## Cambios aplicados

### 1. Platform Gate — Páginas públicas
- **Antes:** Todas las páginas pedían login (redirect a login).
- **Ahora:** Dashboard, login, conoce-la-plataforma, citizen-membership, servicios-renta, nuestra-historia, gran-alianza, todo-propio → **públicas**.

### 2. Barra de navegación unificada (`unified-nav.js`)
- Barra fija arriba con: **IERAHKWA | Dashboard | GOV | BANK | AI | Security | Gaming**.
- Enlaces absolutos que funcionan.
- Se carga automáticamente en todas las páginas que usan `unified-core.js`.

### 3. Footer unificado
- Dashboard + idioma (ES, EN, Taino, Kanien'kéha).
- Sin clutter de Casino/Lotto/Social.

### 4. Script de tests
- `node scripts/production-test.js` — prueba 13 endpoints críticos.

## Cómo probar

```bash
# 1. Arrancar servidor
./start.sh
# o: cd RuddieSolution/node && node server.js

# 2. Tests
cd RuddieSolution/node && node scripts/production-test.js

# 3. Abrir en navegador
open http://localhost:8545/platform/index.html
```

## Páginas públicas (sin login)
- `/platform` — Dashboard principal
- `/platform/login.html` — Login
- `/platform/conoce-la-plataforma.html`
- `/platform/citizen-membership.html`
- `/platform/servicios-renta.html`
- Otras plataformas requieren login (admin/admin123).

## Producción checklist
- [ ] Servidor en 8545
- [ ] `node scripts/production-test.js` → OK
- [ ] Dashboard carga sin login
- [ ] Nav bar visible y enlaces funcionan
- [ ] Login admin/admin123 funciona
- [ ] CORS, SSL según despliegue
