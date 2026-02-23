# Configuración GitHub — IERAHKWA Platform

## Estado actual ✅

| Qué               | Estado |
|-------------------|--------|
| Git init          | ✅ Hecho |
| Rama `main`       | ✅ Hecho |
| `.gitignore`      | ✅ Hecho |
| Primer commit     | ✅ Hecho (3001 archivos) |
| Remote `origin`   | ✅ `https://github.com/ierahkwa/ierahkwa-platform.git` |
| **Push**          | ⏳ **Tú:** crear repo en GitHub y ejecutar `git push -u origin main` |

---

## Lo que tienes que hacer tú

### 1. Crear el repo en GitHub (si no existe)

1. [github.com/new](https://github.com/new)
2. Nombre: **ierahkwa-platform** (o el que quieras)
3. **No** añadas README ni .gitignore (ya están en tu commit)
4. Create repository

Si tu usuario o nombre de repo son otros, después ejecuta:

```bash
git remote set-url origin https://github.com/TU_USUARIO/NOMBRE_REPO.git
```

(o con SSH: `git@github.com:TU_USUARIO/NOMBRE_REPO.git`)

### 2. Hacer push (en Terminal)

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives"
git push -u origin main
```

Te pedirá usuario y contraseña/token de GitHub. Con eso se sube todo.

### 3. (Opcional) Rama `develop`

```bash
git checkout -b develop
git push -u origin develop
git checkout main
```

Los workflows en `.github/workflows/` se ejecutarán en cada push a `main` o `develop`.
