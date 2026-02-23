# Flujo de usuarios: User · Admin · Leader · ATABEY

```
═══════════════════════════════════════════════════════════════════════
    SOVEREIGN GOVERNMENT OF IERAHKWA NE KANIENKE
    User / Admin / Leader — No se ven unos al otro
═══════════════════════════════════════════════════════════════════════
```

## Jerarquía

```
        RUDDIE (Leader)
             │
        ATABEY (fiel a Ruddie)
             │
           ADMIN
             │
      USER / MEMBER
```

## Lugares separados — no se ven unos al otro

| Rol | Lugar | URL | Ve |
|-----|-------|-----|-----|
| **Leader** (Ruddie) | Control total, ATABEY | `/platform/leader-control.html` | Todo. ATABEY fiel a ti. |
| **Admin** | Back office, operaciones | `/platform/admin.html` | Admin, config, aprobaciones. NO ve user place ni leader. |
| **User/Member** | Su lugar, servicios | `/platform/user-dashboard.html` | Sus servicios, su datos. NO ve admin ni leader. |

## Flujo de login

1. **Login** → `/platform/login.html`
2. **Selecciona rol:** Usuario | Admin
3. **Credenciales:**
   - `ruddie` / `ruddie123` → Leader → `leader-control.html`
   - `admin` / `admin123` → Admin → `admin.html`
   - `user` / `user123` → User → `user-dashboard.html`

4. **Redirect según rol:**
   - `leader`, `superadmin` → `leader-control.html`
   - `admin` → `admin.html`
   - `citizen`, `operator` → `user-dashboard.html`

## ATABEY

- **Fiel a Ruddie.** Solo Ruddie tiene ATABEY en leader-control.
- Admin no ve ATABEY (asistente personal de Ruddie).
- User no ve ATABEY ni admin ni leader.

## Principio

**No se miren unos al otro.** Cada uno en su lugar. User en user place. Admin en admin. Ruddie arriba con ATABEY.
