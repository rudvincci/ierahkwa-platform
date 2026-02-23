# Atabey Fortress — Live Command Center

The Fortress is no longer a blueprint; it is a living, breathing digital organism on your own hardware.

---

## Final steps to seal sovereignty

### 1. Set the Tables in the Great Census (:8080)

In **Baserow** (http://localhost:8080), create your first **Database** named:

**"Ierahkwa Sovereign Registry"**

Suggested columns:

| Column              | Purpose                    |
|---------------------|----------------------------|
| Full Name           | Citizen name               |
| Bloodline / Tribe   | Nation or community        |
| Ancestral Territory | Traditional territory      |
| Sovereign Wallet ID | Custodian wallet / ID      |
| Status              | e.g. Custodian, Pending    |

This is where the 12,847 citizens (and the millions coming in) transition from invisible to **Owners of the Continent**.

---

### 2. Secure the Vault (:8081)

In **Nextcloud** (http://localhost:8081):

1. Create the folder **"Grandfather's Archive"**.
2. First uploads:
   - The 1710 Treaties
   - The Two Row Wampum digital render
   - The 2026 Constitution
3. **Encryption:** Enable **Server-side Encryption** in Nextcloud settings.  
   Even if someone physically takes the drive, they cannot read the files without the Atabey Key.

---

### 3. Verify the Guardian Pulse

Run:

```bash
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
```

You want to see: **atabey_database**, **atabey_cloud**, **atabey_messenger**, and **atabey_guardian** all reporting **Up** (and where applicable, **healthy**).

---

## Commander Update — Service status

| Service   | Protocol | Status                    |
|-----------|----------|---------------------------|
| Census    | Baserow  | READY FOR REGISTRATION    |
| Vault     | Nextcloud| WAITING FOR TREATIES      |
| Messenger | Synapse  | VIBRATING AT 432Hz        |
| Guardian  | Wazuh    | WATCHING THE PERIMETER    |
| Gateway   | Nginx PM | SOVEREIGN SHIELD (80/443)|

---

## Clone (repositorio y datos)

Para clonar el repo en otra máquina o restaurar datos desde un backup, ver **[CLONE-SETUP.md](./CLONE-SETUP.md)**. Scripts: `scripts/clone-repo.sh`, `scripts/clone-from-backup.sh`.

---

## Sovereign Backup (nightly bunker clone)

Use the script to clone databases and vault to local backup (and optionally to an offline bunker):

```bash
chmod +x sovereign-backup.sh
./sovereign-backup.sh
```

**Cron (every night at 2:00 AM):**

```bash
0 2 * * * cd /path/to/soberanos\ natives && ./sovereign-backup.sh
```

**Optional:** set an offline bunker path (e.g. external drive) so each run also copies there:

```bash
export SOVEREIGN_BUNKER_PATH="/Volumes/AtabeyBunker"
./sovereign-backup.sh
```

Backups are stored under `sovereign_backups/atabey_YYYY-MM-DD_HH-MM/`. Old backups are pruned after 30 days (configurable via `SOVEREIGN_BACKUP_KEEP_DAYS`).

---

*The Eagle, Quetzal, and Condor have their own digital home now. No more "text"—this is Life Live. One Love.*
