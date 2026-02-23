# Onboarding por rol (Front office, Back office, Leader)

Sovereign Government of Ierahkwa Ne Kanienke

---

## Rol: Usuario / Ciudadano (Front office)

- [ ] Acceso: `login.html` → usuario con rol **citizen** o **operator**.
- [ ] Tras login: redirección a **user-dashboard.html** o **citizen-portal.html**.
- [ ] Qué puede hacer: wallet, documentos, votación, notificaciones, servicios públicos. No ve Admin ni ATABEY.
- [ ] Referencia: `platform/user-dashboard.html`, `platform/citizen-portal.html`, `PLANO-ATABEY-ARRIBA-DE-TODO.md` (sección Front Office).

---

## Rol: Admin Trabajadores (Back office)

- [ ] Acceso: `login.html` → usuario con rol **admin**.
- [ ] Tras login: redirección a **admin.html** (Admin Trabajadores · Back Office).
- [ ] Qué puede hacer: config, roles, gestión de plataformas y empleados, back office. No ve Leader Control ni ATABEY como líder.
- [ ] Si necesita “todo habla conmigo”: no es este rol; ese es Leader. Admin gestiona operación, no el control estratégico.
- [ ] Referencia: `platform/admin.html`, `PLANO-ATABEY-ARRIBA-DE-TODO.md` (sección Back Office).

---

## Rol: Leader (ATABEY)

- [ ] Acceso: `login.html` → usuario con rol **leader** o **superadmin** (o login biométrico).
- [ ] Tras login: redirección a **atabey-platform.html** (ATABEY — todo habla conmigo).
- [ ] Qué puede hacer: Vista Global, AI, Security Fortress, Face, Watchlist, Quantum, Vigilancia, Eventos, Notificaciones, Backup, Cumplimiento, Node, Servicio Inteligencia. Leader Control y Mi Admin.
- [ ] No debe usar admin.html para “su” admin; ATABEY es su centro. Admin es para trabajadores.
- [ ] Referencia: `platform/atabey-platform.html`, `platform/leader-control.html`, `PLANO-ATABEY-ARRIBA-DE-TODO.md`, `docs/REPORTE-POR-QUE-ES-MEJOR-Y-HASTA-DONDE.md`.

---

## Común a todos

- [ ] Conocer principio **Todo propio** (`PRINCIPIO-TODO-PROPIO.md`): sin depender de terceros (Google, AWS, etc.).
- [ ] Producción: credenciales y JWT en backend; ver `PRODUCTION-SETUP.md` y `/api/platform-auth/login`.
