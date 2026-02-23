#!/bin/bash
# ═══════════════════════════════════════════════════════════════════════════════
#  IERAHKWA CRON BACKUP SETUP
#  Configura cron jobs para backups automáticos de bases de datos
# ═══════════════════════════════════════════════════════════════════════════════

set -e

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
BACKUP_SCRIPT="$ROOT/scripts/backup-database.sh"

echo "╔══════════════════════════════════════════════════════════════════════════════╗"
echo "║     ⏰ IERAHKWA CRON BACKUP SETUP                                             ║"
echo "╚══════════════════════════════════════════════════════════════════════════════╝"
echo ""

# Make backup script executable
chmod +x "$BACKUP_SCRIPT"

# Create cron entry
CRON_JOB="0 2 * * * $BACKUP_SCRIPT >> $ROOT/logs/backup-cron.log 2>&1"

# Check if cron job already exists
if crontab -l 2>/dev/null | grep -q "$BACKUP_SCRIPT"; then
    echo "⚠️  Cron job already exists. Updating..."
    crontab -l 2>/dev/null | grep -v "$BACKUP_SCRIPT" | crontab -
fi

# Add new cron job
(crontab -l 2>/dev/null; echo "$CRON_JOB") | crontab -

echo "✅ Cron job configured:"
echo "   Schedule: Daily at 2:00 AM"
echo "   Script: $BACKUP_SCRIPT"
echo "   Log: $ROOT/logs/backup-cron.log"
echo ""
echo "📋 Current cron jobs:"
crontab -l
echo ""
echo "💡 To remove: crontab -e (then delete the line)"
echo "💡 To test: $BACKUP_SCRIPT"
