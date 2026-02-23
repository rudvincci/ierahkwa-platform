# Cómo recuperar un backup encriptado

## Crear el backup (en tu Mac)

**Opción fácil:** doble clic en **`Backup-Encriptado.command`** → escribe la contraseña dos veces → se crea el archivo `.tar.enc`.

O en Terminal:

```bash
./backup-encrypted.sh
```

Te pedirá una **contraseña** (dos veces). El archivo `ierahkwa-backup-encrypted-FECHA.tar.enc` queda cifrado con AES-256; sin esa contraseña nadie puede abrirlo.

## Recuperar en otra carpeta o en otro Mac

```bash
openssl enc -d -aes-256-cbc -in ierahkwa-backup-encrypted-XXXXXXXX.tar.enc -out recuperado.tar -pass prompt
tar xf recuperado.tar
```

Te pedirá la misma contraseña. Se crea la carpeta con todo el proyecto.

---

**Recomendación:** Además de este backup, usa **FileVault** en tu Mac (Preferencias del Sistema → Privacidad y seguridad → FileVault) para tener todo el disco cifrado.
