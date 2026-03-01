#!/usr/bin/env bash
# ============================================================
# harden-server.sh — Ierahkwa Sovereign Server Hardening
# Gobierno Soberano de Ierahkwa Ne Kanienke
# ============================================================
# Usage: sudo ./harden-server.sh
# ============================================================
# This script applies production-grade hardening to sovereign
# infrastructure nodes. It configures SSH, firewall, fail2ban,
# kernel parameters, Docker isolation, audit logging, malware
# scanning, file integrity monitoring, encrypted DNS, Tor
# hidden services, and automatic security updates.
# ============================================================

set -euo pipefail

# -----------------------------------------------------------
# Colors and utilities
# -----------------------------------------------------------
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
PURPLE='\033[0;35m'
NC='\033[0m'
BOLD='\033[1m'
DIM='\033[2m'

log_step()  { echo -e "\n${PURPLE}${BOLD}>>> $1${NC}"; }
log_ok()    { echo -e "${GREEN}[OK]${NC}    $1"; }
log_warn()  { echo -e "${YELLOW}[WARN]${NC}  $1"; }
log_error() { echo -e "${RED}[ERROR]${NC} $1"; }
log_info()  { echo -e "${CYAN}[INFO]${NC}  $1"; }

# Track all changes for final report
declare -a CHANGES_MADE=()
track_change() { CHANGES_MADE+=("$1"); }

TIMESTAMP="$(date +%Y%m%d_%H%M%S)"
BACKUP_DIR="/root/ierahkwa-hardening-backup-${TIMESTAMP}"
LOG_FILE="/var/log/ierahkwa-hardening-${TIMESTAMP}.log"

# -----------------------------------------------------------
# Banner
# -----------------------------------------------------------
echo -e "${PURPLE}${BOLD}"
echo "  ========================================================"
echo "   IERAHKWA NE KANIENKE — Sovereign Server Hardening"
echo "   Gobierno Soberano — Nacion Digital"
echo "   19 Naciones | 72M Personas | 574 Tribus"
echo "  ========================================================"
echo -e "${NC}"
echo -e "${DIM}  Timestamp: $(date -u '+%Y-%m-%d %H:%M:%S UTC')${NC}"
echo -e "${DIM}  Hostname:  $(hostname)${NC}"
echo -e "${DIM}  Kernel:    $(uname -r)${NC}"
echo ""

# -----------------------------------------------------------
# 0. Root check
# -----------------------------------------------------------
log_step "0/16 — Verifying root privileges"

if [[ $EUID -ne 0 ]]; then
    log_error "This script must be run as root (sudo ./harden-server.sh)"
    exit 1
fi
log_ok "Running as root (UID $EUID)"

# Create backup directory
mkdir -p "$BACKUP_DIR"
log_ok "Backup directory created: $BACKUP_DIR"

# Start logging
exec > >(tee -a "$LOG_FILE") 2>&1
log_info "Logging to $LOG_FILE"

# Detect package manager
if command -v apt-get &>/dev/null; then
    PKG_MGR="apt"
    PKG_INSTALL="apt-get install -y -qq"
    PKG_UPDATE="apt-get update -qq"
elif command -v dnf &>/dev/null; then
    PKG_MGR="dnf"
    PKG_INSTALL="dnf install -y -q"
    PKG_UPDATE="dnf check-update -q || true"
elif command -v yum &>/dev/null; then
    PKG_MGR="yum"
    PKG_INSTALL="yum install -y -q"
    PKG_UPDATE="yum check-update -q || true"
else
    log_error "Unsupported package manager. Requires apt, dnf, or yum."
    exit 1
fi
log_ok "Package manager detected: $PKG_MGR"

log_info "Updating package index..."
$PKG_UPDATE
log_ok "Package index updated"

# -----------------------------------------------------------
# 1. SSH Hardening
# -----------------------------------------------------------
log_step "1/16 — Hardening SSH configuration"

SSHD_CONFIG="/etc/ssh/sshd_config"
SSHD_BACKUP="${BACKUP_DIR}/sshd_config.bak"

if [[ -f "$SSHD_CONFIG" ]]; then
    cp "$SSHD_CONFIG" "$SSHD_BACKUP"
    log_ok "Backed up sshd_config to $SSHD_BACKUP"

    # Create hardened sshd_config drop-in
    mkdir -p /etc/ssh/sshd_config.d
    cat > /etc/ssh/sshd_config.d/00-ierahkwa-hardening.conf <<'SSHEOF'
# Ierahkwa Sovereign SSH Hardening
# Applied by harden-server.sh

# Change default port
Port 2222

# Disable root login
PermitRootLogin no

# Disable password authentication (key-only)
PasswordAuthentication no
ChallengeResponseAuthentication no
UsePAM yes

# Only allow strong key types
PubkeyAcceptedKeyTypes ssh-ed25519,ssh-ed25519-cert-v01@openssh.com,rsa-sha2-512,rsa-sha2-256
HostKeyAlgorithms ssh-ed25519,ssh-ed25519-cert-v01@openssh.com,rsa-sha2-512,rsa-sha2-256

# Strict session parameters
LoginGraceTime 30
MaxAuthTries 3
MaxSessions 5
ClientAliveInterval 300
ClientAliveCountMax 2

# Disable X11 and agent forwarding
X11Forwarding no
AllowAgentForwarding no
AllowTcpForwarding no

# Disable empty passwords
PermitEmptyPasswords no

# Use strong ciphers and MACs only
Ciphers chacha20-poly1305@openssh.com,aes256-gcm@openssh.com,aes128-gcm@openssh.com
MACs hmac-sha2-512-etm@openssh.com,hmac-sha2-256-etm@openssh.com

# Key exchange algorithms
KexAlgorithms curve25519-sha256,curve25519-sha256@libssh.org,diffie-hellman-group16-sha512,diffie-hellman-group18-sha512

# Logging
LogLevel VERBOSE

# Restrict to protocol 2
Protocol 2

# Banner
Banner /etc/ssh/banner
SSHEOF

    # Create SSH banner
    cat > /etc/ssh/banner <<'BANNEREOF'
╔══════════════════════════════════════════════════════════╗
║  IERAHKWA NE KANIENKE — SOVEREIGN DIGITAL TERRITORY     ║
║  Unauthorized access is a violation of sovereign law.    ║
║  All sessions are monitored, logged, and audited.        ║
║  Proceed only if authorized by the Sovereign Council.    ║
╚══════════════════════════════════════════════════════════╝
BANNEREOF

    # Validate config before restarting
    if sshd -t -f "$SSHD_CONFIG" 2>/dev/null; then
        systemctl restart sshd 2>/dev/null || systemctl restart ssh 2>/dev/null || true
        log_ok "SSH hardened: port 2222, key-only auth, ed25519/rsa, LoginGraceTime 30s"
        track_change "SSH: Port 2222, root login disabled, password auth disabled, strong ciphers only"
    else
        log_warn "SSH config validation failed — restoring backup"
        cp "$SSHD_BACKUP" "$SSHD_CONFIG"
        rm -f /etc/ssh/sshd_config.d/00-ierahkwa-hardening.conf
    fi
else
    log_warn "sshd_config not found — SSH hardening skipped"
fi

# -----------------------------------------------------------
# 2. Firewall (UFW)
# -----------------------------------------------------------
log_step "2/16 — Configuring UFW firewall"

if ! command -v ufw &>/dev/null; then
    log_info "Installing UFW..."
    $PKG_INSTALL ufw
fi

# Reset UFW to clean state
ufw --force reset >/dev/null 2>&1

# Default policies
ufw default deny incoming
ufw default allow outgoing
log_ok "Default policy: deny incoming, allow outgoing"

# Allow SSH on new port
ufw allow 2222/tcp comment 'Ierahkwa SSH'
log_ok "Allowed 2222/tcp (SSH)"

# HTTP/HTTPS
ufw allow 80/tcp comment 'HTTP'
ufw allow 443/tcp comment 'HTTPS'
log_ok "Allowed 80/tcp (HTTP), 443/tcp (HTTPS)"

# MameyNode blockchain
ufw allow 8545/tcp comment 'MameyNode RPC'
log_ok "Allowed 8545/tcp (MameyNode blockchain)"

# Sovereign-core API
ufw allow 3050/tcp comment 'Sovereign Core API'
log_ok "Allowed 3050/tcp (sovereign-core)"

# LoRa mesh gateway
ufw allow 1680/udp comment 'LoRa Mesh Gateway'
log_ok "Allowed 1680/udp (LoRa mesh)"

# Rate limiting on SSH
ufw limit 2222/tcp comment 'SSH rate limit'

# Enable UFW
ufw --force enable
log_ok "UFW enabled and configured"
track_change "UFW: default deny, allowed 2222/tcp, 80/tcp, 443/tcp, 8545/tcp, 3050/tcp, 1680/udp"

# -----------------------------------------------------------
# 3. Fail2Ban
# -----------------------------------------------------------
log_step "3/16 — Installing and configuring Fail2Ban"

$PKG_INSTALL fail2ban

# Backup existing config
if [[ -f /etc/fail2ban/jail.local ]]; then
    cp /etc/fail2ban/jail.local "${BACKUP_DIR}/jail.local.bak"
fi

cat > /etc/fail2ban/jail.local <<'F2BEOF'
# Ierahkwa Sovereign Fail2Ban Configuration

[DEFAULT]
bantime  = 3600
findtime = 600
maxretry = 3
backend  = systemd
banaction = ufw

# Email notifications (if configured)
destemail = security@ierahkwa.sovereign
sender    = fail2ban@ierahkwa.sovereign
action    = %(action_mwl)s

[sshd]
enabled  = true
port     = 2222
filter   = sshd
logpath  = /var/log/auth.log
maxretry = 3
bantime  = 3600
findtime = 600

[sshd-ddos]
enabled  = true
port     = 2222
filter   = sshd-ddos
logpath  = /var/log/auth.log
maxretry = 6
bantime  = 7200

[nginx-http-auth]
enabled  = true
port     = http,https
filter   = nginx-http-auth
logpath  = /var/log/nginx/error.log
maxretry = 3
bantime  = 3600

[nginx-botsearch]
enabled  = true
port     = http,https
filter   = nginx-botsearch
logpath  = /var/log/nginx/access.log
maxretry = 2
bantime  = 86400

[docker-daemon]
enabled  = true
port     = 2375,2376
filter   = docker-daemon
logpath  = /var/log/docker.log
maxretry = 3
bantime  = 7200

[recidive]
enabled  = true
filter   = recidive
logpath  = /var/log/fail2ban.log
bantime  = 604800
findtime = 86400
maxretry = 3
F2BEOF

# Create custom Docker filter
mkdir -p /etc/fail2ban/filter.d
cat > /etc/fail2ban/filter.d/docker-daemon.conf <<'DOCKERFILTER'
# Ierahkwa Docker Daemon filter for Fail2Ban
[Definition]
failregex = ^.*Unauthorized attempt to connect from <HOST>.*$
            ^.*denied connection from <HOST>.*$
            ^.*authentication failure.*rhost=<HOST>.*$
ignoreregex =
DOCKERFILTER

systemctl enable fail2ban
systemctl restart fail2ban
log_ok "Fail2Ban configured: sshd (3 retries/1h ban), nginx, docker-daemon, recidive"
track_change "Fail2Ban: sshd jail, nginx-http-auth, docker-daemon filter, recidive jail"

# -----------------------------------------------------------
# 4. Kernel Hardening (sysctl)
# -----------------------------------------------------------
log_step "4/16 — Applying kernel hardening via sysctl"

SYSCTL_CONF="/etc/sysctl.d/99-ierahkwa-hardening.conf"
if [[ -f "$SYSCTL_CONF" ]]; then
    cp "$SYSCTL_CONF" "${BACKUP_DIR}/99-ierahkwa-hardening.conf.bak"
fi

cat > "$SYSCTL_CONF" <<'SYSCTL'
# Ierahkwa Sovereign Kernel Hardening
# Applied by harden-server.sh

# --- Network Security ---

# Enable SYN cookies (SYN flood protection)
net.ipv4.tcp_syncookies = 1

# Disable ICMP redirects (prevent MITM)
net.ipv4.conf.all.accept_redirects = 0
net.ipv4.conf.default.accept_redirects = 0
net.ipv4.conf.all.send_redirects = 0
net.ipv4.conf.default.send_redirects = 0
net.ipv6.conf.all.accept_redirects = 0
net.ipv6.conf.default.accept_redirects = 0

# Enable reverse path filtering (anti-spoofing)
net.ipv4.conf.all.rp_filter = 1
net.ipv4.conf.default.rp_filter = 1

# Ignore ICMP broadcast requests
net.ipv4.icmp_echo_ignore_broadcasts = 1

# Ignore bogus ICMP error responses
net.ipv4.icmp_ignore_bogus_error_responses = 1

# Log martian packets
net.ipv4.conf.all.log_martians = 1
net.ipv4.conf.default.log_martians = 1

# Disable source routing
net.ipv4.conf.all.accept_source_route = 0
net.ipv4.conf.default.accept_source_route = 0
net.ipv6.conf.all.accept_source_route = 0
net.ipv6.conf.default.accept_source_route = 0

# Disable IP forwarding (re-enable if Docker needs it)
# Docker overrides this via its own iptables rules
net.ipv4.ip_forward = 0

# TCP hardening
net.ipv4.tcp_max_syn_backlog = 4096
net.ipv4.tcp_synack_retries = 2
net.ipv4.tcp_syn_retries = 3
net.ipv4.tcp_fin_timeout = 15
net.ipv4.tcp_keepalive_time = 300
net.ipv4.tcp_keepalive_probes = 3
net.ipv4.tcp_keepalive_intvl = 15

# --- IPv6 (disable if not in use) ---
net.ipv6.conf.all.disable_ipv6 = 1
net.ipv6.conf.default.disable_ipv6 = 1
net.ipv6.conf.lo.disable_ipv6 = 1

# --- Memory Protection ---

# Restrict dmesg access
kernel.dmesg_restrict = 1

# Restrict kernel pointers in /proc
kernel.kptr_restrict = 2

# Enable ASLR (Address Space Layout Randomization)
kernel.randomize_va_space = 2

# Restrict access to perf events
kernel.perf_event_paranoid = 3

# Restrict ptrace (process tracing)
kernel.yama.ptrace_scope = 2

# Restrict core dumps
fs.suid_dumpable = 0

# Restrict unprivileged BPF
kernel.unprivileged_bpf_disabled = 1

# Restrict userfaultfd to privileged users
vm.unprivileged_userfaultfd = 0

# Restrict kernel module loading after boot
# kernel.modules_disabled = 1  # Uncomment after all modules loaded

# --- File System ---

# Protect symlinks and hardlinks
fs.protected_symlinks = 1
fs.protected_hardlinks = 1
fs.protected_fifos = 2
fs.protected_regular = 2
SYSCTL

sysctl --system >/dev/null 2>&1
log_ok "Kernel hardened: SYN cookies, anti-spoofing, ASLR, ptrace restricted, IPv6 disabled"
track_change "Kernel: SYN cookies, rp_filter, no ICMP redirects, IPv6 disabled, ASLR, ptrace restricted"

# -----------------------------------------------------------
# 5. Docker Isolation
# -----------------------------------------------------------
log_step "5/16 — Hardening Docker configuration"

if command -v docker &>/dev/null; then
    DOCKER_DAEMON="/etc/docker/daemon.json"
    mkdir -p /etc/docker

    if [[ -f "$DOCKER_DAEMON" ]]; then
        cp "$DOCKER_DAEMON" "${BACKUP_DIR}/daemon.json.bak"
    fi

    cat > "$DOCKER_DAEMON" <<'DOCKERJSON'
{
    "icc": false,
    "userns-remap": "default",
    "no-new-privileges": true,
    "log-driver": "json-file",
    "log-opts": {
        "max-size": "10m",
        "max-file": "3"
    },
    "storage-driver": "overlay2",
    "live-restore": true,
    "userland-proxy": false,
    "default-ulimits": {
        "nofile": {
            "Name": "nofile",
            "Hard": 65536,
            "Soft": 32768
        },
        "nproc": {
            "Name": "nproc",
            "Hard": 4096,
            "Soft": 2048
        }
    },
    "default-address-pools": [
        {
            "base": "172.30.0.0/16",
            "size": 24
        }
    ],
    "seccomp-profile": "/etc/docker/seccomp-ierahkwa.json",
    "default-runtime": "runc",
    "runtimes": {
        "runc": {
            "path": "runc"
        }
    }
}
DOCKERJSON

    # Create custom seccomp profile (based on Docker default, more restrictive)
    cat > /etc/docker/seccomp-ierahkwa.json <<'SECCOMP'
{
    "defaultAction": "SCMP_ACT_ERRNO",
    "defaultErrnoRet": 1,
    "archMap": [
        { "architecture": "SCMP_ARCH_X86_64", "subArchitectures": ["SCMP_ARCH_X86", "SCMP_ARCH_X32"] },
        { "architecture": "SCMP_ARCH_AARCH64", "subArchitectures": ["SCMP_ARCH_ARM"] }
    ],
    "syscalls": [
        {
            "names": [
                "accept", "accept4", "access", "adjtimex", "alarm", "bind",
                "brk", "capget", "capset", "chdir", "chmod", "chown",
                "clock_getres", "clock_gettime", "clock_nanosleep", "clone",
                "close", "connect", "copy_file_range", "creat", "dup", "dup2",
                "dup3", "epoll_create", "epoll_create1", "epoll_ctl",
                "epoll_pwait", "epoll_wait", "eventfd", "eventfd2",
                "execve", "execveat", "exit", "exit_group", "faccessat",
                "fadvise64", "fallocate", "fanotify_mark", "fchdir", "fchmod",
                "fchmodat", "fchown", "fchownat", "fcntl", "fdatasync",
                "fgetxattr", "flistxattr", "flock", "fork", "fsetxattr",
                "fstat", "fstatfs", "fsync", "ftruncate", "futex",
                "futimesat", "getcwd", "getdents", "getdents64", "getegid",
                "geteuid", "getgid", "getgroups", "getitimer", "getpeername",
                "getpgid", "getpgrp", "getpid", "getppid", "getpriority",
                "getrandom", "getresgid", "getresuid", "getrlimit",
                "getrusage", "getsid", "getsockname", "getsockopt",
                "get_thread_area", "gettid", "gettimeofday", "getuid",
                "getxattr", "inotify_add_watch", "inotify_init",
                "inotify_init1", "inotify_rm_watch", "io_cancel",
                "io_destroy", "io_getevents", "ioprio_get", "ioprio_set",
                "io_setup", "io_submit", "kill", "lchown", "lgetxattr",
                "link", "linkat", "listen", "listxattr", "llistxattr",
                "lseek", "lsetxattr", "lstat", "madvise", "membarrier",
                "memfd_create", "mincore", "mkdir", "mkdirat", "mknod",
                "mknodat", "mlock", "mlock2", "mlockall", "mmap", "mount",
                "mprotect", "mq_getsetattr", "mq_notify", "mq_open",
                "mq_timedreceive", "mq_timedsend", "mq_unlink", "mremap",
                "msgctl", "msgget", "msgrcv", "msgsnd", "msync", "munlock",
                "munlockall", "munmap", "nanosleep", "newfstatat", "open",
                "openat", "pause", "pipe", "pipe2", "poll", "ppoll",
                "prctl", "pread64", "preadv", "preadv2", "prlimit64",
                "pselect6", "pwrite64", "pwritev", "pwritev2", "read",
                "readahead", "readlink", "readlinkat", "readv", "recv",
                "recvfrom", "recvmmsg", "recvmsg", "remap_file_pages",
                "removexattr", "rename", "renameat", "renameat2", "restart_syscall",
                "rmdir", "rt_sigaction", "rt_sigpending", "rt_sigprocmask",
                "rt_sigqueueinfo", "rt_sigreturn", "rt_sigsuspend",
                "rt_sigtimedwait", "rt_tgsigqueueinfo", "sched_getaffinity",
                "sched_getattr", "sched_getparam", "sched_get_priority_max",
                "sched_get_priority_min", "sched_getscheduler",
                "sched_setaffinity", "sched_setattr", "sched_setparam",
                "sched_setscheduler", "sched_yield", "seccomp", "select",
                "semctl", "semget", "semop", "semtimedop", "send",
                "sendfile", "sendmmsg", "sendmsg", "sendto", "setfsgid",
                "setfsuid", "setgid", "setgroups", "setitimer", "setpgid",
                "setpriority", "setregid", "setresgid", "setresuid",
                "setreuid", "setrlimit", "set_robust_list", "setsid",
                "setsockopt", "set_thread_area", "set_tid_address",
                "setuid", "shmat", "shmctl", "shmdt", "shmget", "shutdown",
                "sigaltstack", "signalfd", "signalfd4", "socket",
                "socketpair", "splice", "stat", "statfs", "statx",
                "symlink", "symlinkat", "sync", "sync_file_range",
                "syncfs", "sysinfo", "tee", "tgkill", "time",
                "timer_create", "timer_delete", "timerfd_create",
                "timerfd_gettime", "timerfd_settime", "timer_getoverrun",
                "timer_gettime", "timer_settime", "times", "tkill",
                "truncate", "umask", "uname", "unlink", "unlinkat",
                "utime", "utimensat", "utimes", "vfork", "vmsplice",
                "wait4", "waitid", "waitpid", "write", "writev"
            ],
            "action": "SCMP_ACT_ALLOW"
        }
    ]
}
SECCOMP

    # Create dedicated sovereign Docker network
    docker network create \
        --driver bridge \
        --subnet 172.30.0.0/24 \
        --gateway 172.30.0.1 \
        --opt com.docker.network.bridge.enable_icc=false \
        --opt com.docker.network.bridge.enable_ip_masquerade=true \
        ierahkwa-sovereign 2>/dev/null || log_warn "Network 'ierahkwa-sovereign' already exists"

    # Restrict Docker socket permissions
    chmod 660 /var/run/docker.sock 2>/dev/null || true

    # Re-enable IP forwarding for Docker (it needs it)
    sysctl -w net.ipv4.ip_forward=1 >/dev/null 2>&1

    systemctl restart docker 2>/dev/null || true
    log_ok "Docker hardened: user namespaces, no-new-privileges, ICC disabled, custom seccomp"
    track_change "Docker: user namespaces, no-new-privileges, ICC disabled, ulimits, custom seccomp profile"
else
    log_warn "Docker not installed — Docker hardening skipped"
fi

# -----------------------------------------------------------
# 6. Automatic Security Updates
# -----------------------------------------------------------
log_step "6/16 — Configuring automatic security updates"

if [[ "$PKG_MGR" == "apt" ]]; then
    $PKG_INSTALL unattended-upgrades apt-listchanges

    cat > /etc/apt/apt.conf.d/50unattended-upgrades <<'UUEOF'
Unattended-Upgrade::Allowed-Origins {
    "${distro_id}:${distro_codename}-security";
    "${distro_id}ESMApps:${distro_codename}-apps-security";
    "${distro_id}ESM:${distro_codename}-infra-security";
};
Unattended-Upgrade::AutoFixInterruptedDpkg "true";
Unattended-Upgrade::MinimalSteps "true";
Unattended-Upgrade::Remove-Unused-Kernel-Packages "true";
Unattended-Upgrade::Remove-Unused-Dependencies "true";
Unattended-Upgrade::Automatic-Reboot "false";
Unattended-Upgrade::Mail "security@ierahkwa.sovereign";
Unattended-Upgrade::MailReport "on-change";
UUEOF

    cat > /etc/apt/apt.conf.d/20auto-upgrades <<'AUTOEOF'
APT::Periodic::Update-Package-Lists "1";
APT::Periodic::Unattended-Upgrade "1";
APT::Periodic::Download-Upgradeable-Packages "1";
APT::Periodic::AutocleanInterval "7";
AUTOEOF

    systemctl enable unattended-upgrades
    systemctl restart unattended-upgrades
    log_ok "Unattended-upgrades configured (security-only, no auto-reboot)"
    track_change "Auto-updates: unattended-upgrades enabled for security patches"

elif [[ "$PKG_MGR" == "dnf" ]] || [[ "$PKG_MGR" == "yum" ]]; then
    $PKG_INSTALL dnf-automatic 2>/dev/null || $PKG_INSTALL yum-cron 2>/dev/null || true

    if [[ -f /etc/dnf/automatic.conf ]]; then
        sed -i 's/^apply_updates.*/apply_updates = yes/' /etc/dnf/automatic.conf
        sed -i 's/^upgrade_type.*/upgrade_type = security/' /etc/dnf/automatic.conf
        systemctl enable --now dnf-automatic.timer 2>/dev/null || true
        log_ok "dnf-automatic configured for security updates"
        track_change "Auto-updates: dnf-automatic enabled for security patches"
    fi
fi

# -----------------------------------------------------------
# 7. Audit Logging (auditd)
# -----------------------------------------------------------
log_step "7/16 — Installing and configuring audit logging"

$PKG_INSTALL auditd audispd-plugins 2>/dev/null || $PKG_INSTALL audit 2>/dev/null || true

AUDIT_RULES="/etc/audit/rules.d/ierahkwa-sovereign.rules"

cat > "$AUDIT_RULES" <<'AUDITEOF'
# Ierahkwa Sovereign Audit Rules
# Applied by harden-server.sh

# Delete all existing rules
-D

# Set buffer size
-b 8192

# Failure mode: 1=printk, 2=panic
-f 1

# ---- File System Monitoring ----

# Monitor /etc for configuration changes
-w /etc/ -p wa -k etc_changes

# Monitor /etc/passwd and /etc/shadow
-w /etc/passwd -p wa -k identity
-w /etc/shadow -p wa -k identity
-w /etc/group -p wa -k identity
-w /etc/gshadow -p wa -k identity
-w /etc/sudoers -p wa -k sudoers
-w /etc/sudoers.d/ -p wa -k sudoers

# Monitor SSH configuration
-w /etc/ssh/ -p wa -k ssh_config
-w /etc/ssh/sshd_config -p wa -k ssh_config

# ---- Authentication Events ----

# Login/logout events
-w /var/log/auth.log -p wa -k auth_log
-w /var/log/faillog -p wa -k faillog
-w /var/log/lastlog -p wa -k lastlog
-w /var/log/wtmp -p wa -k wtmp
-w /var/log/btmp -p wa -k btmp

# PAM configuration
-w /etc/pam.d/ -p wa -k pam_config

# ---- Privilege Escalation ----

# Sudo usage
-w /usr/bin/sudo -p x -k sudo_usage
-w /usr/bin/su -p x -k su_usage

# Setuid/setgid execution
-a always,exit -F arch=b64 -S execve -F euid=0 -F auid>=1000 -F auid!=4294967295 -k priv_escalation
-a always,exit -F arch=b32 -S execve -F euid=0 -F auid>=1000 -F auid!=4294967295 -k priv_escalation

# ---- Docker Monitoring ----

# Docker socket access
-w /var/run/docker.sock -p rwxa -k docker_socket
-w /usr/bin/docker -p x -k docker_cmd
-w /usr/bin/dockerd -p x -k docker_daemon
-w /etc/docker/ -p wa -k docker_config

# ---- Mount Operations ----

# Mount/unmount operations
-a always,exit -F arch=b64 -S mount -S umount2 -k mount_ops
-a always,exit -F arch=b32 -S mount -S umount -S umount2 -k mount_ops

# ---- Network Monitoring ----

# Network configuration changes
-w /etc/hosts -p wa -k network_hosts
-w /etc/resolv.conf -p wa -k network_dns
-w /etc/network/ -p wa -k network_config
-w /etc/netplan/ -p wa -k network_config

# Socket creation
-a always,exit -F arch=b64 -S socket -F a0=2 -k network_socket_ipv4
-a always,exit -F arch=b64 -S socket -F a0=10 -k network_socket_ipv6

# ---- System Time ----

# Time changes
-a always,exit -F arch=b64 -S adjtimex -S settimeofday -S clock_settime -k time_change
-w /etc/localtime -p wa -k time_change

# ---- Kernel Module Loading ----

-w /sbin/insmod -p x -k kernel_modules
-w /sbin/rmmod -p x -k kernel_modules
-w /sbin/modprobe -p x -k kernel_modules
-a always,exit -F arch=b64 -S init_module -S delete_module -k kernel_modules

# ---- Cron Jobs ----

-w /etc/crontab -p wa -k cron
-w /etc/cron.d/ -p wa -k cron
-w /etc/cron.daily/ -p wa -k cron
-w /etc/cron.hourly/ -p wa -k cron
-w /etc/cron.monthly/ -p wa -k cron
-w /etc/cron.weekly/ -p wa -k cron
-w /var/spool/cron/ -p wa -k cron

# Make rules immutable (requires reboot to change)
-e 2
AUDITEOF

# Restart auditd
systemctl enable auditd
service auditd restart 2>/dev/null || systemctl restart auditd 2>/dev/null || auditctl -R "$AUDIT_RULES" 2>/dev/null || true
log_ok "Auditd configured: /etc changes, auth events, sudo, Docker socket, mounts, kernel modules"
track_change "Auditd: 30+ audit rules for filesystem, auth, sudo, Docker, network, kernel modules"

# -----------------------------------------------------------
# 8. ClamAV Antivirus
# -----------------------------------------------------------
log_step "8/16 — Installing and configuring ClamAV"

$PKG_INSTALL clamav clamav-daemon clamav-freshclam 2>/dev/null || \
$PKG_INSTALL clamav clamav-update clamd 2>/dev/null || true

if command -v freshclam &>/dev/null; then
    # Stop freshclam if running to update database
    systemctl stop clamav-freshclam 2>/dev/null || true
    freshclam --quiet 2>/dev/null || log_warn "freshclam update failed (may need network)"

    systemctl enable clamav-freshclam 2>/dev/null || true
    systemctl start clamav-freshclam 2>/dev/null || true
    systemctl enable clamav-daemon 2>/dev/null || true
    systemctl start clamav-daemon 2>/dev/null || true

    # Create daily scan cron job
    cat > /etc/cron.daily/ierahkwa-clamscan <<'CLAMCRON'
#!/bin/bash
# Ierahkwa Daily ClamAV Scan
SCAN_DIRS="/home /tmp /var/www /var/lib/docker/volumes"
LOG="/var/log/clamav/ierahkwa-daily-scan.log"
mkdir -p /var/log/clamav

echo "=== Ierahkwa ClamAV Scan: $(date -u) ===" >> "$LOG"

for DIR in $SCAN_DIRS; do
    if [[ -d "$DIR" ]]; then
        clamscan -r --infected --no-summary "$DIR" >> "$LOG" 2>&1
    fi
done

# Count infected files
INFECTED=$(grep -c "FOUND" "$LOG" 2>/dev/null || echo "0")
if [[ "$INFECTED" -gt 0 ]]; then
    echo "ALERT: $INFECTED infected file(s) found! Check $LOG" | \
        logger -t ierahkwa-clamscan -p auth.crit
fi

echo "=== Scan complete: $INFECTED threats found ===" >> "$LOG"
CLAMCRON
    chmod +x /etc/cron.daily/ierahkwa-clamscan

    log_ok "ClamAV installed with daily scans of /home, /tmp, /var/www, Docker volumes"
    track_change "ClamAV: installed, daily scans of /home, /tmp, /var/www, Docker volumes"
else
    log_warn "ClamAV installation failed — skipping"
fi

# -----------------------------------------------------------
# 9. Rootkit Detection
# -----------------------------------------------------------
log_step "9/16 — Installing rootkit detection tools"

$PKG_INSTALL rkhunter 2>/dev/null || true
$PKG_INSTALL chkrootkit 2>/dev/null || true

if command -v rkhunter &>/dev/null; then
    # Update rkhunter database
    rkhunter --update --quiet 2>/dev/null || true
    rkhunter --propupd --quiet 2>/dev/null || true

    # Configure rkhunter
    if [[ -f /etc/rkhunter.conf ]]; then
        cp /etc/rkhunter.conf "${BACKUP_DIR}/rkhunter.conf.bak"
        sed -i 's/^CRON_DAILY_RUN=.*/CRON_DAILY_RUN="true"/' /etc/rkhunter.conf 2>/dev/null || true
        sed -i 's/^CRON_DB_UPDATE=.*/CRON_DB_UPDATE="true"/' /etc/rkhunter.conf 2>/dev/null || true
    fi

    log_ok "rkhunter installed and database updated"
    track_change "rkhunter: installed, daily scans enabled"
fi

if command -v chkrootkit &>/dev/null; then
    # Create weekly chkrootkit cron
    cat > /etc/cron.weekly/ierahkwa-chkrootkit <<'CHKCRON'
#!/bin/bash
# Ierahkwa Weekly chkrootkit Scan
LOG="/var/log/ierahkwa-chkrootkit-$(date +%Y%m%d).log"
chkrootkit > "$LOG" 2>&1
INFECTED=$(grep -c "INFECTED" "$LOG" 2>/dev/null || echo "0")
if [[ "$INFECTED" -gt 0 ]]; then
    echo "ALERT: chkrootkit found $INFECTED suspicious item(s)! Check $LOG" | \
        logger -t ierahkwa-chkrootkit -p auth.crit
fi
CHKCRON
    chmod +x /etc/cron.weekly/ierahkwa-chkrootkit
    log_ok "chkrootkit installed with weekly scans"
    track_change "chkrootkit: installed, weekly scans enabled"
fi

# -----------------------------------------------------------
# 10. File Integrity Monitoring (AIDE)
# -----------------------------------------------------------
log_step "10/16 — Installing AIDE (file integrity monitoring)"

$PKG_INSTALL aide 2>/dev/null || $PKG_INSTALL aide aide-common 2>/dev/null || true

if command -v aide &>/dev/null || command -v aide.wrapper &>/dev/null; then
    AIDE_CONF="/etc/aide/aide.conf"
    AIDE_CMD="aide"

    # Use aide.wrapper on Debian/Ubuntu if available
    if command -v aide.wrapper &>/dev/null; then
        AIDE_CMD="aide.wrapper"
    fi

    # Add sovereign-specific paths to monitor
    if [[ -f "$AIDE_CONF" ]]; then
        cp "$AIDE_CONF" "${BACKUP_DIR}/aide.conf.bak"
    fi

    # Create custom AIDE config additions
    mkdir -p /etc/aide/aide.conf.d
    cat > /etc/aide/aide.conf.d/99-ierahkwa.conf <<'AIDEEOF'
# Ierahkwa Sovereign File Integrity Rules

# Docker configuration
/etc/docker CONTENT_EX
/etc/docker/daemon.json CONTENT_EX

# SSH keys and config
/etc/ssh CONTENT_EX

# Sovereign application directories
/opt/ierahkwa CONTENT_EX

# Firewall rules
/etc/ufw CONTENT_EX

# Fail2Ban config
/etc/fail2ban CONTENT_EX

# Cron jobs
/etc/cron.d CONTENT_EX
/etc/cron.daily CONTENT_EX
/etc/crontab CONTENT_EX

# Systemd services
/etc/systemd CONTENT_EX

# Binaries
/usr/local/bin CONTENT_EX
/usr/local/sbin CONTENT_EX
AIDEEOF

    # Initialize AIDE database
    log_info "Initializing AIDE database (this may take several minutes)..."
    aideinit 2>/dev/null || $AIDE_CMD --init 2>/dev/null || true

    # Move new database into place
    if [[ -f /var/lib/aide/aide.db.new ]]; then
        cp /var/lib/aide/aide.db.new /var/lib/aide/aide.db
    elif [[ -f /var/lib/aide/aide.db.new.gz ]]; then
        cp /var/lib/aide/aide.db.new.gz /var/lib/aide/aide.db.gz
    fi

    # Create daily check cron
    cat > /etc/cron.daily/ierahkwa-aide-check <<'AIDECRON'
#!/bin/bash
# Ierahkwa Daily AIDE Integrity Check
LOG="/var/log/ierahkwa-aide-$(date +%Y%m%d).log"
AIDE_CMD="aide"
command -v aide.wrapper &>/dev/null && AIDE_CMD="aide.wrapper"

echo "=== Ierahkwa AIDE Check: $(date -u) ===" > "$LOG"
$AIDE_CMD --check >> "$LOG" 2>&1
RESULT=$?

if [[ $RESULT -ne 0 ]]; then
    echo "ALERT: AIDE detected filesystem changes! Check $LOG" | \
        logger -t ierahkwa-aide -p auth.crit
fi
AIDECRON
    chmod +x /etc/cron.daily/ierahkwa-aide-check

    log_ok "AIDE initialized with daily integrity checks"
    track_change "AIDE: file integrity monitoring initialized, daily checks configured"
else
    log_warn "AIDE installation failed — skipping"
fi

# -----------------------------------------------------------
# 11. DNS-over-HTTPS
# -----------------------------------------------------------
log_step "11/16 — Configuring encrypted DNS (DNS-over-HTTPS)"

# Try cloudflared first, fall back to systemd-resolved DoT
if ! command -v cloudflared &>/dev/null; then
    # Attempt to install cloudflared
    if [[ "$PKG_MGR" == "apt" ]]; then
        # Add Cloudflare GPG key and repo
        curl -fsSL https://pkg.cloudflare.com/cloudflare-main.gpg 2>/dev/null | \
            tee /usr/share/keyrings/cloudflare-main.gpg >/dev/null 2>&1 || true
        echo "deb [signed-by=/usr/share/keyrings/cloudflare-main.gpg] https://pkg.cloudflare.com/cloudflared $(lsb_release -cs 2>/dev/null || echo 'jammy') main" | \
            tee /etc/apt/sources.list.d/cloudflared.list >/dev/null 2>&1 || true
        apt-get update -qq 2>/dev/null || true
        $PKG_INSTALL cloudflared 2>/dev/null || true
    fi
fi

if command -v cloudflared &>/dev/null; then
    # Configure cloudflared as DNS proxy
    mkdir -p /etc/cloudflared
    cat > /etc/cloudflared/config.yml <<'CFEOF'
# Ierahkwa Sovereign Encrypted DNS Configuration
proxy-dns: true
proxy-dns-port: 5053
proxy-dns-upstream:
  - https://1.1.1.1/dns-query
  - https://1.0.0.1/dns-query
  - https://9.9.9.9/dns-query
  - https://149.112.112.112/dns-query
CFEOF

    # Create systemd service for cloudflared DNS
    cat > /etc/systemd/system/cloudflared-dns.service <<'CFSVC'
[Unit]
Description=Ierahkwa Sovereign DNS-over-HTTPS (cloudflared)
After=network.target

[Service]
Type=simple
ExecStart=/usr/bin/cloudflared --config /etc/cloudflared/config.yml
Restart=on-failure
RestartSec=10
User=nobody
Group=nogroup
AmbientCapabilities=CAP_NET_BIND_SERVICE

[Install]
WantedBy=multi-user.target
CFSVC

    systemctl daemon-reload
    systemctl enable cloudflared-dns
    systemctl start cloudflared-dns 2>/dev/null || true

    # Point system DNS to local cloudflared
    if [[ -f /etc/resolv.conf ]]; then
        cp /etc/resolv.conf "${BACKUP_DIR}/resolv.conf.bak"
    fi

    # Configure systemd-resolved to use cloudflared
    mkdir -p /etc/systemd/resolved.conf.d
    cat > /etc/systemd/resolved.conf.d/ierahkwa-dns.conf <<'DNSCONF'
[Resolve]
DNS=127.0.0.1#5053
FallbackDNS=1.1.1.1#853 9.9.9.9#853
DNSOverTLS=opportunistic
DNSSEC=allow-downgrade
DNSCONF

    systemctl restart systemd-resolved 2>/dev/null || true
    log_ok "DNS-over-HTTPS configured via cloudflared (Cloudflare + Quad9)"
    track_change "DNS: encrypted DNS via cloudflared (DoH with Cloudflare 1.1.1.1 + Quad9 9.9.9.9)"
else
    # Fallback: configure systemd-resolved for DNS-over-TLS
    if systemctl is-active systemd-resolved &>/dev/null 2>&1; then
        mkdir -p /etc/systemd/resolved.conf.d
        cat > /etc/systemd/resolved.conf.d/ierahkwa-dns.conf <<'DOTCONF'
[Resolve]
DNS=1.1.1.1#cloudflare-dns.com 9.9.9.9#dns.quad9.net
FallbackDNS=1.0.0.1#cloudflare-dns.com 149.112.112.112#dns.quad9.net
DNSOverTLS=yes
DNSSEC=allow-downgrade
DOTCONF

        systemctl restart systemd-resolved
        log_ok "DNS-over-TLS configured via systemd-resolved (Cloudflare + Quad9)"
        track_change "DNS: DNS-over-TLS via systemd-resolved"
    else
        log_warn "Neither cloudflared nor systemd-resolved available — encrypted DNS skipped"
    fi
fi

# -----------------------------------------------------------
# 12. NTP Time Synchronization
# -----------------------------------------------------------
log_step "12/16 — Configuring NTP time synchronization"

$PKG_INSTALL chrony 2>/dev/null || $PKG_INSTALL ntp 2>/dev/null || true

if command -v chronyd &>/dev/null; then
    CHRONY_CONF="/etc/chrony/chrony.conf"
    [[ ! -f "$CHRONY_CONF" ]] && CHRONY_CONF="/etc/chrony.conf"

    if [[ -f "$CHRONY_CONF" ]]; then
        cp "$CHRONY_CONF" "${BACKUP_DIR}/chrony.conf.bak"
    fi

    cat > "$CHRONY_CONF" <<'NTPEOF'
# Ierahkwa Sovereign NTP Configuration
# Multiple sovereign-friendly, privacy-respecting NTP servers

# Primary pools
pool time.cloudflare.com iburst nts
pool time.google.com iburst
pool pool.ntp.org iburst

# Stratum 1 servers (diverse geography)
server time1.google.com iburst
server time2.google.com iburst
server time.nist.gov iburst
server ntp.ubuntu.com iburst

# Allow local network sync
allow 172.30.0.0/24
allow 10.0.0.0/8

# Record tracking statistics
driftfile /var/lib/chrony/drift
makestep 1.0 3
rtcsync

# Enable kernel timestamping for best accuracy
hwtimestamp *

# Increase minimum number of selectable sources
minsources 3

# Log statistics
logdir /var/log/chrony
log tracking measurements statistics

# Restrict cmdmon to localhost
cmdallow 127.0.0.1
cmdallow ::1
NTPEOF

    systemctl enable chrony 2>/dev/null || systemctl enable chronyd 2>/dev/null || true
    systemctl restart chrony 2>/dev/null || systemctl restart chronyd 2>/dev/null || true
    log_ok "Chrony NTP configured with multiple sovereign-friendly servers"
    track_change "NTP: Chrony with Cloudflare, Google, NIST, Ubuntu time servers"

elif command -v ntpd &>/dev/null; then
    systemctl enable ntp
    systemctl restart ntp
    log_ok "NTP daemon configured"
    track_change "NTP: ntpd configured with default servers"
fi

# -----------------------------------------------------------
# 13. MOTD Banner
# -----------------------------------------------------------
log_step "13/16 — Setting sovereign login banner"

cat > /etc/motd <<'MOTDEOF'

╔═══════════════════════════════════════════════════════════════════╗
║                                                                   ║
║   ██╗███████╗██████╗  █████╗ ██╗  ██╗██╗  ██╗██╗    ██╗ █████╗  ║
║   ██║██╔════╝██╔══██╗██╔══██╗██║  ██║██║ ██╔╝██║    ██║██╔══██╗ ║
║   ██║█████╗  ██████╔╝███████║███████║█████╔╝ ██║ █╗ ██║███████║ ║
║   ██║██╔══╝  ██╔══██╗██╔══██║██╔══██║██╔═██╗ ██║███╗██║██╔══██║ ║
║   ██║███████╗██║  ██║██║  ██║██║  ██║██║  ██╗╚███╔███╔╝██║  ██║ ║
║   ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚══╝╚══╝ ╚═╝  ╚═╝ ║
║                                                                   ║
║            IERAHKWA NE KANIENKE — SOVEREIGN TERRITORY             ║
║            Gobierno Soberano — Nacion Digital Indigena             ║
║                                                                   ║
║   WARNING: This system is the property of the Sovereign Digital   ║
║   Nation of Ierahkwa Ne Kanienke. Unauthorized access, use, or    ║
║   modification is strictly prohibited and constitutes a           ║
║   violation of sovereign law.                                     ║
║                                                                   ║
║   All activity is monitored, recorded, and subject to audit.      ║
║   Violators will be prosecuted to the fullest extent of           ║
║   applicable sovereign and international law.                     ║
║                                                                   ║
║   19 Naciones | 574 Tribus | 72M Personas                        ║
║   MameyNode Blockchain | Guardian AI | Zero Trust                 ║
║                                                                   ║
╚═══════════════════════════════════════════════════════════════════╝

MOTDEOF

# Also set /etc/issue for pre-login
cat > /etc/issue <<'ISSUEEOF'
Ierahkwa Ne Kanienke — Sovereign Territory
Unauthorized access is a violation of sovereign law.
All sessions are monitored and audited.

ISSUEEOF

log_ok "Sovereign MOTD and pre-login banner configured"
track_change "Banner: /etc/motd and /etc/issue set with sovereign warnings"

# -----------------------------------------------------------
# 14. Tor Hidden Service
# -----------------------------------------------------------
log_step "14/16 — Configuring Tor hidden service"

$PKG_INSTALL tor 2>/dev/null || true

if command -v tor &>/dev/null; then
    TORRC="/etc/tor/torrc"
    if [[ -f "$TORRC" ]]; then
        cp "$TORRC" "${BACKUP_DIR}/torrc.bak"
    fi

    # Create hidden service directory
    mkdir -p /var/lib/tor/ierahkwa-sovereign
    chown debian-tor:debian-tor /var/lib/tor/ierahkwa-sovereign 2>/dev/null || \
    chown toranon:toranon /var/lib/tor/ierahkwa-sovereign 2>/dev/null || \
    chown tor:tor /var/lib/tor/ierahkwa-sovereign 2>/dev/null || true
    chmod 700 /var/lib/tor/ierahkwa-sovereign

    # Configure hidden service
    local TOR_CONF_CONTENT
    read -r -d '' TOR_CONF_CONTENT <<'TOREOF' || true
# Ierahkwa Sovereign Tor Hidden Service Configuration

# Hidden service for sovereign web portal
HiddenServiceDir /var/lib/tor/ierahkwa-sovereign/
HiddenServiceVersion 3
HiddenServicePort 80 127.0.0.1:80
HiddenServicePort 443 127.0.0.1:443

# Security hardening
SocksPort 0
RunAsDaemon 1

# Bandwidth and circuit settings
MaxCircuitDirtiness 600
CircuitBuildTimeout 30

# Logging
Log notice file /var/log/tor/notices.log
TOREOF

    mkdir -p /etc/tor/torrc.d 2>/dev/null || true
    if [[ -d /etc/tor/torrc.d ]]; then
        echo "$TOR_CONF_CONTENT" > /etc/tor/torrc.d/ierahkwa-hidden-service
    else
        echo "$TOR_CONF_CONTENT" >> "$TORRC"
    fi

    mkdir -p /var/log/tor
    chown debian-tor:debian-tor /var/log/tor 2>/dev/null || \
    chown tor:tor /var/log/tor 2>/dev/null || true

    systemctl enable tor
    systemctl restart tor 2>/dev/null || true

    # Wait briefly for onion address generation
    sleep 3

    ONION_ADDR=""
    if [[ -f /var/lib/tor/ierahkwa-sovereign/hostname ]]; then
        ONION_ADDR=$(cat /var/lib/tor/ierahkwa-sovereign/hostname 2>/dev/null || echo "pending")
        log_ok "Tor hidden service active: $ONION_ADDR"
    else
        log_info "Tor hidden service configured — .onion address will generate on first start"
    fi

    track_change "Tor: hidden service v3 configured for ports 80,443"
else
    log_warn "Tor installation failed — hidden service skipped"
fi

# -----------------------------------------------------------
# 15. Additional Security Measures
# -----------------------------------------------------------
log_step "15/16 — Applying additional security hardening"

# --- Disable unused filesystems ---
log_info "Disabling unused filesystem modules..."
MODPROBE_CONF="/etc/modprobe.d/ierahkwa-disable-filesystems.conf"
cat > "$MODPROBE_CONF" <<'MODEOF'
# Ierahkwa: Disable unused filesystem modules
install cramfs /bin/true
install freevxfs /bin/true
install jffs2 /bin/true
install hfs /bin/true
install hfsplus /bin/true
install squashfs /bin/true
install udf /bin/true
install vfat /bin/true

# Disable unused network protocols
install dccp /bin/true
install sctp /bin/true
install rds /bin/true
install tipc /bin/true

# Disable USB storage (if not needed)
# install usb-storage /bin/true
MODEOF
log_ok "Unused filesystem and network protocol modules disabled"
track_change "Modules: disabled cramfs, freevxfs, jffs2, hfs, hfsplus, dccp, sctp, rds, tipc"

# --- Secure shared memory ---
if ! grep -q "tmpfs.*\/run\/shm" /etc/fstab 2>/dev/null; then
    echo "tmpfs /run/shm tmpfs defaults,noexec,nosuid,nodev 0 0" >> /etc/fstab
    mount -o remount /run/shm 2>/dev/null || true
    log_ok "Shared memory secured (noexec, nosuid, nodev)"
    track_change "Shared memory: /run/shm mounted with noexec,nosuid,nodev"
fi

# --- Restrict core dumps ---
if [[ -d /etc/security/limits.d ]]; then
    echo "* hard core 0" > /etc/security/limits.d/ierahkwa-no-core.conf
    log_ok "Core dumps disabled"
    track_change "Core dumps: disabled globally"
fi

# --- Disable uncommon network services ---
for SVC in avahi-daemon cups bluetooth rpcbind nfs-common portmap; do
    if systemctl is-active "$SVC" &>/dev/null 2>&1; then
        systemctl stop "$SVC"
        systemctl disable "$SVC"
        log_ok "Disabled service: $SVC"
        track_change "Service: disabled $SVC"
    fi
done

# --- Restrict permissions on critical files ---
chmod 600 /etc/shadow 2>/dev/null || true
chmod 600 /etc/gshadow 2>/dev/null || true
chmod 644 /etc/passwd 2>/dev/null || true
chmod 644 /etc/group 2>/dev/null || true
chmod 600 /boot/grub/grub.cfg 2>/dev/null || true
log_ok "Critical file permissions tightened"
track_change "Permissions: restricted /etc/shadow, /etc/gshadow, /boot/grub/grub.cfg"

# --- Set umask ---
if [[ -f /etc/login.defs ]]; then
    sed -i 's/^UMASK.*/UMASK 027/' /etc/login.defs
    log_ok "Default umask set to 027"
    track_change "Umask: set to 027 in /etc/login.defs"
fi

# --- Password policy ---
$PKG_INSTALL libpam-pwquality 2>/dev/null || $PKG_INSTALL pam_pwquality 2>/dev/null || true
if [[ -f /etc/security/pwquality.conf ]]; then
    cat > /etc/security/pwquality.conf <<'PWEOF'
# Ierahkwa Sovereign Password Policy
minlen = 14
dcredit = -1
ucredit = -1
ocredit = -1
lcredit = -1
minclass = 4
maxrepeat = 3
maxclassrepeat = 4
gecoscheck = 1
dictcheck = 1
enforcing = 1
PWEOF
    log_ok "Password policy enforced: min 14 chars, all character classes required"
    track_change "Password policy: min 14 chars, complexity requirements via pwquality"
fi

# --- Secure cron ---
if [[ -f /etc/cron.allow ]] || [[ ! -f /etc/cron.deny ]]; then
    echo "root" > /etc/cron.allow
    chmod 600 /etc/cron.allow
    rm -f /etc/cron.deny
    log_ok "Cron restricted to root only"
    track_change "Cron: restricted to root user only"
fi

# --- Secure at ---
if [[ -f /etc/at.allow ]] || command -v at &>/dev/null; then
    echo "root" > /etc/at.allow
    chmod 600 /etc/at.allow
    rm -f /etc/at.deny
    log_ok "at restricted to root only"
    track_change "at: restricted to root user only"
fi

# -----------------------------------------------------------
# 16. Summary Report
# -----------------------------------------------------------
log_step "16/16 — Hardening Summary Report"

echo ""
echo -e "${PURPLE}${BOLD}╔═══════════════════════════════════════════════════════════╗${NC}"
echo -e "${PURPLE}${BOLD}║     IERAHKWA SOVEREIGN SERVER HARDENING — COMPLETE       ║${NC}"
echo -e "${PURPLE}${BOLD}╚═══════════════════════════════════════════════════════════╝${NC}"
echo ""

echo -e "${CYAN}${BOLD}Timestamp:${NC} $(date -u '+%Y-%m-%d %H:%M:%S UTC')"
echo -e "${CYAN}${BOLD}Hostname:${NC}  $(hostname)"
echo -e "${CYAN}${BOLD}Kernel:${NC}    $(uname -r)"
echo -e "${CYAN}${BOLD}Log file:${NC}  $LOG_FILE"
echo -e "${CYAN}${BOLD}Backups:${NC}   $BACKUP_DIR"
echo ""

echo -e "${GREEN}${BOLD}Changes Applied (${#CHANGES_MADE[@]} total):${NC}"
echo -e "${DIM}──────────────────────────────────────────────────${NC}"
for i in "${!CHANGES_MADE[@]}"; do
    echo -e "  ${GREEN}[$((i+1))]${NC} ${CHANGES_MADE[$i]}"
done
echo ""

# Service status
echo -e "${CYAN}${BOLD}Service Status:${NC}"
echo -e "${DIM}──────────────────────────────────────────────────${NC}"
for SVC in sshd ssh ufw fail2ban auditd clamav-daemon clamav-freshclam tor chrony chronyd cloudflared-dns; do
    if systemctl is-active "$SVC" &>/dev/null 2>&1; then
        echo -e "  ${GREEN}[ACTIVE]${NC}   $SVC"
    elif systemctl is-enabled "$SVC" &>/dev/null 2>&1; then
        echo -e "  ${YELLOW}[ENABLED]${NC}  $SVC (not yet active)"
    fi
done
echo ""

# Firewall status
echo -e "${CYAN}${BOLD}Firewall Rules:${NC}"
echo -e "${DIM}──────────────────────────────────────────────────${NC}"
ufw status numbered 2>/dev/null | head -20 || echo "  UFW status unavailable"
echo ""

# Fail2Ban status
echo -e "${CYAN}${BOLD}Fail2Ban Jails:${NC}"
echo -e "${DIM}──────────────────────────────────────────────────${NC}"
fail2ban-client status 2>/dev/null || echo "  Fail2Ban status unavailable"
echo ""

# Tor hidden service
if [[ -n "${ONION_ADDR:-}" ]] && [[ "$ONION_ADDR" != "pending" ]]; then
    echo -e "${CYAN}${BOLD}Tor Hidden Service:${NC}"
    echo -e "${DIM}──────────────────────────────────────────────────${NC}"
    echo -e "  ${GREEN}Onion:${NC} $ONION_ADDR"
    echo ""
fi

# Important reminders
echo -e "${YELLOW}${BOLD}IMPORTANT REMINDERS:${NC}"
echo -e "${DIM}──────────────────────────────────────────────────${NC}"
echo -e "  ${YELLOW}[!]${NC} SSH is now on port ${BOLD}2222${NC} — update your client config!"
echo -e "  ${YELLOW}[!]${NC} Password auth is ${BOLD}DISABLED${NC} — ensure SSH keys are deployed"
echo -e "  ${YELLOW}[!]${NC} Root login is ${BOLD}DISABLED${NC} — use a regular user with sudo"
echo -e "  ${YELLOW}[!]${NC} IPv6 is ${BOLD}DISABLED${NC} — enable in sysctl if needed"
echo -e "  ${YELLOW}[!]${NC} Audit rules are ${BOLD}IMMUTABLE${NC} — reboot required to change"
echo -e "  ${YELLOW}[!]${NC} Docker IP forwarding re-enabled for container networking"
echo -e "  ${YELLOW}[!]${NC} Run ${BOLD}aide --check${NC} periodically to verify file integrity"
echo -e "  ${YELLOW}[!]${NC} Backups saved to: ${BOLD}$BACKUP_DIR${NC}"
echo ""

echo -e "${PURPLE}${BOLD}══════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}${BOLD}  Sovereign infrastructure hardened successfully.${NC}"
echo -e "${PURPLE}${BOLD}══════════════════════════════════════════════════════════${NC}"
echo ""

exit 0
