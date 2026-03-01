#!/usr/bin/env bash
# =============================================================================
# Ierahkwa Sovereign DNS - Configuracion de Handshake (HNS)
# =============================================================================
#
# Este script instala y configura un nodo completo de Handshake (hsd) para
# resolver el TLD soberano .ierahkwa sin depender de ICANN ni de ningun
# proveedor de DNS centralizado.
#
# Handshake es un protocolo descentralizado de nombrado que reemplaza las
# autoridades de certificacion y el sistema DNS raiz con un blockchain
# basado en prueba de trabajo. Esto permite a la red Ierahkwa controlar
# su propio espacio de nombres de dominio de forma completamente soberana.
#
# Requisitos previos:
#   - Sistema operativo: Linux (Ubuntu 22.04+ recomendado) o macOS
#   - Node.js 18+ y npm
#   - Git
#   - Al menos 50 GB de disco para la cadena de bloques HNS
#   - Acceso root/sudo para configurar el resolver DNS local
#
# Uso:
#   chmod +x setup_hns.sh
#   sudo ./setup_hns.sh
#
# =============================================================================

set -euo pipefail

# --- Colores para salida legible ---
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # Sin color

# --- Configuracion ---
HSD_VERSION="v6.0.0"
HSD_INSTALL_DIR="/opt/ierahkwa/hsd"
HSD_DATA_DIR="/var/lib/ierahkwa/hsd"
HSD_LOG_DIR="/var/log/ierahkwa/hsd"
HSD_CONFIG_FILE="/etc/ierahkwa/handshake-config.json"
HSD_USER="ierahkwa-dns"
HSD_GROUP="ierahkwa-dns"
LOCAL_DNS_PORT=5350
RECURSIVE_DNS_PORT=53
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# --- Funciones auxiliares ---

log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[OK]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[AVISO]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

check_root() {
    if [[ $EUID -ne 0 ]]; then
        log_error "Este script debe ejecutarse como root (use sudo)."
        exit 1
    fi
}

detect_os() {
    # Detectar el sistema operativo para ajustar comandos de instalacion
    if [[ -f /etc/os-release ]]; then
        . /etc/os-release
        OS_ID="${ID}"
        OS_VERSION="${VERSION_ID}"
    elif [[ "$(uname)" == "Darwin" ]]; then
        OS_ID="macos"
        OS_VERSION="$(sw_vers -productVersion)"
    else
        log_error "Sistema operativo no soportado."
        exit 1
    fi
    log_info "Sistema detectado: ${OS_ID} ${OS_VERSION}"
}

# --- Paso 1: Verificar e instalar dependencias del sistema ---

install_system_dependencies() {
    log_info "Paso 1/8: Verificando dependencias del sistema..."

    # Verificar Node.js
    if ! command -v node &> /dev/null; then
        log_warning "Node.js no encontrado. Instalando..."
        if [[ "${OS_ID}" == "ubuntu" || "${OS_ID}" == "debian" ]]; then
            curl -fsSL https://deb.nodesource.com/setup_20.x | bash -
            apt-get install -y nodejs
        elif [[ "${OS_ID}" == "macos" ]]; then
            if command -v brew &> /dev/null; then
                brew install node
            else
                log_error "Homebrew no encontrado. Instale Node.js manualmente."
                exit 1
            fi
        else
            log_error "Instale Node.js 18+ manualmente para su sistema operativo."
            exit 1
        fi
    fi

    NODE_VERSION=$(node --version | sed 's/v//' | cut -d. -f1)
    if [[ "${NODE_VERSION}" -lt 18 ]]; then
        log_error "Se requiere Node.js 18+. Version actual: $(node --version)"
        exit 1
    fi
    log_success "Node.js $(node --version) disponible."

    # Verificar Git
    if ! command -v git &> /dev/null; then
        log_warning "Git no encontrado. Instalando..."
        if [[ "${OS_ID}" == "ubuntu" || "${OS_ID}" == "debian" ]]; then
            apt-get install -y git
        elif [[ "${OS_ID}" == "macos" ]]; then
            xcode-select --install 2>/dev/null || true
        fi
    fi
    log_success "Git $(git --version | cut -d' ' -f3) disponible."

    # Instalar herramientas de compilacion necesarias para los modulos nativos de hsd
    if [[ "${OS_ID}" == "ubuntu" || "${OS_ID}" == "debian" ]]; then
        apt-get install -y build-essential python3 libunbound-dev
    fi

    log_success "Dependencias del sistema verificadas."
}

# --- Paso 2: Crear usuario y directorios del sistema ---

setup_system_user() {
    log_info "Paso 2/8: Configurando usuario y directorios del sistema..."

    # Crear usuario del sistema sin shell de login para seguridad
    if ! id "${HSD_USER}" &>/dev/null; then
        if [[ "${OS_ID}" == "macos" ]]; then
            # macOS usa dscl para crear usuarios del sistema
            dscl . -create /Users/${HSD_USER}
            dscl . -create /Users/${HSD_USER} UserShell /usr/bin/false
            dscl . -create /Users/${HSD_USER} UniqueID 510
            dscl . -create /Users/${HSD_USER} PrimaryGroupID 510
        else
            useradd --system --no-create-home --shell /usr/bin/false \
                --user-group "${HSD_USER}"
        fi
        log_success "Usuario del sistema '${HSD_USER}' creado."
    else
        log_info "Usuario '${HSD_USER}' ya existe."
    fi

    # Crear directorios necesarios
    mkdir -p "${HSD_INSTALL_DIR}"
    mkdir -p "${HSD_DATA_DIR}"
    mkdir -p "${HSD_LOG_DIR}"
    mkdir -p "$(dirname "${HSD_CONFIG_FILE}")"

    # Establecer permisos restrictivos
    chown -R "${HSD_USER}:${HSD_GROUP}" "${HSD_DATA_DIR}"
    chown -R "${HSD_USER}:${HSD_GROUP}" "${HSD_LOG_DIR}"
    chmod 750 "${HSD_DATA_DIR}"
    chmod 750 "${HSD_LOG_DIR}"

    log_success "Directorios del sistema configurados."
}

# --- Paso 3: Instalar Handshake (hsd) desde GitHub ---

install_hsd() {
    log_info "Paso 3/8: Instalando Handshake daemon (hsd) ${HSD_VERSION}..."

    if [[ -d "${HSD_INSTALL_DIR}/node_modules" ]]; then
        log_info "hsd ya esta instalado. Verificando version..."
        CURRENT_VERSION=$(cd "${HSD_INSTALL_DIR}" && node -e "console.log(require('./package.json').version)" 2>/dev/null || echo "desconocida")
        log_info "Version instalada: ${CURRENT_VERSION}"
    fi

    # Clonar repositorio oficial de Handshake
    if [[ ! -d "${HSD_INSTALL_DIR}/.git" ]]; then
        git clone --depth 1 --branch "${HSD_VERSION}" \
            https://github.com/handshake-org/hsd.git "${HSD_INSTALL_DIR}"
    else
        cd "${HSD_INSTALL_DIR}"
        git fetch --tags
        git checkout "${HSD_VERSION}"
    fi

    # Instalar dependencias de Node.js
    cd "${HSD_INSTALL_DIR}"
    npm install --production

    # Verificar que la instalacion fue exitosa
    if [[ -f "${HSD_INSTALL_DIR}/bin/hsd" ]]; then
        log_success "hsd instalado exitosamente en ${HSD_INSTALL_DIR}"
    else
        log_error "La instalacion de hsd fallo. Verifique los errores anteriores."
        exit 1
    fi

    # Crear enlaces simbolicos para acceso global
    ln -sf "${HSD_INSTALL_DIR}/bin/hsd" /usr/local/bin/hsd
    ln -sf "${HSD_INSTALL_DIR}/bin/hsw-cli" /usr/local/bin/hsw-cli
    ln -sf "${HSD_INSTALL_DIR}/bin/hsd-cli" /usr/local/bin/hsd-cli

    log_success "Enlaces simbolicos creados en /usr/local/bin/"
}

# --- Paso 4: Copiar y aplicar configuracion ---

configure_hsd() {
    log_info "Paso 4/8: Aplicando configuracion de Handshake..."

    # Copiar archivo de configuracion desde el directorio del script
    if [[ -f "${SCRIPT_DIR}/handshake-config.json" ]]; then
        cp "${SCRIPT_DIR}/handshake-config.json" "${HSD_CONFIG_FILE}"
        log_success "Configuracion copiada a ${HSD_CONFIG_FILE}"
    else
        log_warning "No se encontro handshake-config.json en ${SCRIPT_DIR}"
        log_info "Generando configuracion por defecto..."

        cat > "${HSD_CONFIG_FILE}" << 'HSDCONFIG'
{
  "network": "main",
  "prefix": "/var/lib/ierahkwa/hsd",
  "host": "127.0.0.1",
  "port": 12038,
  "ns-host": "127.0.0.1",
  "ns-port": 5350,
  "rs-host": "127.0.0.1",
  "rs-port": 5351,
  "log-level": "info",
  "log-file": true,
  "workers": true,
  "max-inbound": 32,
  "max-outbound": 8,
  "memory": false,
  "compact": true,
  "prune": false
}
HSDCONFIG
    fi

    chown "${HSD_USER}:${HSD_GROUP}" "${HSD_CONFIG_FILE}"
    chmod 640 "${HSD_CONFIG_FILE}"

    log_success "Configuracion de hsd aplicada."
}

# --- Paso 5: Configurar el resolver DNS local ---
# Esto permite que el sistema resuelva dominios .ierahkwa usando el nodo HNS local

configure_local_dns() {
    log_info "Paso 5/8: Configurando resolver DNS local para .ierahkwa..."

    if [[ "${OS_ID}" == "macos" ]]; then
        # macOS: usar /etc/resolver/ para resolver TLDs personalizados
        mkdir -p /etc/resolver

        cat > /etc/resolver/ierahkwa << RESOLVER
# Resolver soberano para el TLD .ierahkwa
# Dirige consultas DNS para *.ierahkwa al nodo Handshake local
nameserver 127.0.0.1
port ${LOCAL_DNS_PORT}
RESOLVER
        log_success "Resolver macOS configurado en /etc/resolver/ierahkwa"

    elif [[ "${OS_ID}" == "ubuntu" || "${OS_ID}" == "debian" ]]; then
        # Linux con systemd-resolved: agregar stub de resolucion para .ierahkwa

        # Crear archivo de configuracion para systemd-resolved
        mkdir -p /etc/systemd/resolved.conf.d

        cat > /etc/systemd/resolved.conf.d/ierahkwa.conf << RESOLVEDCONF
# Configuracion de DNS soberano para la red Ierahkwa
# Redirige consultas del TLD .ierahkwa al nodo Handshake local
[Resolve]
DNS=127.0.0.1#${LOCAL_DNS_PORT}
Domains=~ierahkwa
RESOLVEDCONF

        # Reiniciar systemd-resolved para aplicar cambios
        systemctl restart systemd-resolved
        log_success "systemd-resolved configurado para resolver .ierahkwa"

        # Tambien configurar dnsmasq como alternativa robusta
        if command -v dnsmasq &> /dev/null; then
            cat >> /etc/dnsmasq.d/ierahkwa.conf << DNSMASQCONF
# Redirigir consultas .ierahkwa al nodo Handshake local
server=/ierahkwa/127.0.0.1#${LOCAL_DNS_PORT}
DNSMASQCONF
            systemctl restart dnsmasq 2>/dev/null || true
            log_success "dnsmasq configurado como resolver secundario."
        fi
    fi

    log_success "Resolver DNS local configurado."
}

# --- Paso 6: Crear servicio systemd para ejecucion persistente ---

create_systemd_service() {
    log_info "Paso 6/8: Creando servicio del sistema..."

    if [[ "${OS_ID}" == "macos" ]]; then
        # macOS usa launchd en lugar de systemd
        PLIST_PATH="/Library/LaunchDaemons/com.ierahkwa.hsd.plist"

        cat > "${PLIST_PATH}" << PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN"
  "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Label</key>
    <string>com.ierahkwa.hsd</string>
    <key>ProgramArguments</key>
    <array>
        <string>/usr/local/bin/node</string>
        <string>${HSD_INSTALL_DIR}/bin/hsd</string>
        <string>--config=${HSD_CONFIG_FILE}</string>
    </array>
    <key>RunAtLoad</key>
    <true/>
    <key>KeepAlive</key>
    <true/>
    <key>StandardOutPath</key>
    <string>${HSD_LOG_DIR}/hsd-stdout.log</string>
    <key>StandardErrorPath</key>
    <string>${HSD_LOG_DIR}/hsd-stderr.log</string>
    <key>UserName</key>
    <string>${HSD_USER}</string>
</dict>
</plist>
PLIST
        log_success "LaunchDaemon creado en ${PLIST_PATH}"

    else
        # Linux: crear servicio systemd
        cat > /etc/systemd/system/ierahkwa-hsd.service << SYSTEMD
[Unit]
Description=Ierahkwa Sovereign DNS - Handshake Full Node
Documentation=https://github.com/handshake-org/hsd
After=network-online.target
Wants=network-online.target

[Service]
Type=simple
User=${HSD_USER}
Group=${HSD_GROUP}

# Ejecutar hsd con la configuracion soberana
ExecStart=/usr/local/bin/node ${HSD_INSTALL_DIR}/bin/hsd --config=${HSD_CONFIG_FILE}

# Reiniciar automaticamente si el proceso falla
Restart=on-failure
RestartSec=10

# Limites de recursos
LimitNOFILE=65536
MemoryMax=2G

# Seguridad: restringir acceso al sistema de archivos
ProtectSystem=strict
ProtectHome=true
ReadWritePaths=${HSD_DATA_DIR} ${HSD_LOG_DIR}
PrivateTmp=true
NoNewPrivileges=true

# Logging
StandardOutput=journal
StandardError=journal
SyslogIdentifier=ierahkwa-hsd

[Install]
WantedBy=multi-user.target
SYSTEMD

        # Recargar systemd para reconocer el nuevo servicio
        systemctl daemon-reload
        log_success "Servicio systemd creado: ierahkwa-hsd.service"
    fi
}

# --- Paso 7: Configurar enlace con Matrix/Synapse para comunicaciones soberanas ---
# Matrix/Synapse usa DNS SRV records. El nodo HNS resolvera los dominios .ierahkwa
# que apuntan al servidor Matrix soberano.

configure_matrix_link() {
    log_info "Paso 7/8: Configurando enlace DNS para Matrix/Synapse..."

    # Crear zona DNS local para servicios internos de la red Ierahkwa
    # que seran resueltos por el nodo HNS una vez que el TLD este activo
    ZONES_DIR="${HSD_DATA_DIR}/zones"
    mkdir -p "${ZONES_DIR}"

    cat > "${ZONES_DIR}/ierahkwa-services.zone" << 'ZONE'
; =============================================================================
; Zona DNS para servicios internos de la red Ierahkwa
; Estos registros se resuelven a traves del nodo Handshake local
; =============================================================================

; Servidor Matrix/Synapse para comunicaciones soberanas cifradas
; Registro SRV para que los clientes Matrix descubran el servidor automaticamente
; Formato: _servicio._protocolo.dominio TTL clase tipo prioridad peso puerto objetivo
_matrix._tcp.chat.ierahkwa.    86400 IN SRV 10 100 8448 matrix.ierahkwa.

; Registro para delegacion de servidor Matrix (well-known)
matrix.ierahkwa.               86400 IN A   10.42.0.10

; Portal principal de la red
portal.ierahkwa.               86400 IN A   10.42.0.1

; API de servicios soberanos
api.ierahkwa.                  86400 IN A   10.42.0.2

; Nodo blockchain MameyNode
chain.ierahkwa.                86400 IN A   10.42.0.3

; Registro de identidad soberana
identity.ierahkwa.             86400 IN A   10.42.0.4

; Bio-Ledger ambiental
bioledger.ierahkwa.            86400 IN A   10.42.0.5

; Sistema de votacion descentralizado
vote.ierahkwa.                 86400 IN A   10.42.0.6

; Protocolo Veritas de verificacion
veritas.ierahkwa.              86400 IN A   10.42.0.7

; Red de comunicaciones de emergencia
emergency.ierahkwa.            86400 IN A   10.42.0.8

; Repositorio de codigo soberano
code.ierahkwa.                 86400 IN A   10.42.0.9
ZONE

    chown "${HSD_USER}:${HSD_GROUP}" "${ZONES_DIR}/ierahkwa-services.zone"
    chmod 640 "${ZONES_DIR}/ierahkwa-services.zone"

    log_success "Zona DNS para servicios Ierahkwa configurada."
    log_info "  Matrix/Synapse: chat.ierahkwa -> matrix.ierahkwa:8448"
    log_info "  Portal: portal.ierahkwa -> 10.42.0.1"
    log_info "  API: api.ierahkwa -> 10.42.0.2"
    log_info "  Blockchain: chain.ierahkwa -> 10.42.0.3"
}

# --- Paso 8: Iniciar el servicio y verificar ---

start_and_verify() {
    log_info "Paso 8/8: Iniciando servicio y verificando..."

    if [[ "${OS_ID}" == "macos" ]]; then
        launchctl load /Library/LaunchDaemons/com.ierahkwa.hsd.plist
        sleep 5

        if launchctl list | grep -q "com.ierahkwa.hsd"; then
            log_success "Servicio hsd iniciado correctamente en macOS."
        else
            log_warning "El servicio puede tardar en iniciar. Verifique logs en ${HSD_LOG_DIR}/"
        fi
    else
        # Habilitar e iniciar el servicio systemd
        systemctl enable ierahkwa-hsd.service
        systemctl start ierahkwa-hsd.service

        sleep 5

        if systemctl is-active --quiet ierahkwa-hsd.service; then
            log_success "Servicio ierahkwa-hsd activo y ejecutandose."
        else
            log_warning "El servicio puede tardar en sincronizar la cadena."
            log_info "Verifique el estado con: systemctl status ierahkwa-hsd"
            log_info "Verifique logs con: journalctl -u ierahkwa-hsd -f"
        fi
    fi

    # Verificar que el nodo esta respondiendo
    sleep 2
    if command -v hsd-cli &> /dev/null; then
        log_info "Verificando conectividad del nodo..."
        if hsd-cli info &>/dev/null; then
            log_success "Nodo HNS respondiendo correctamente."
            hsd-cli info 2>/dev/null | head -5 || true
        else
            log_info "El nodo esta iniciando. La sincronizacion inicial puede tomar horas."
        fi
    fi
}

# --- Resumen final ---

print_summary() {
    echo ""
    echo -e "${GREEN}============================================================${NC}"
    echo -e "${GREEN} IERAHKWA SOVEREIGN DNS - Instalacion Completada${NC}"
    echo -e "${GREEN}============================================================${NC}"
    echo ""
    echo -e "  Instalacion hsd:     ${HSD_INSTALL_DIR}"
    echo -e "  Datos blockchain:    ${HSD_DATA_DIR}"
    echo -e "  Logs:                ${HSD_LOG_DIR}"
    echo -e "  Configuracion:       ${HSD_CONFIG_FILE}"
    echo -e "  Puerto DNS:          ${LOCAL_DNS_PORT}"
    echo -e "  Usuario del sistema: ${HSD_USER}"
    echo ""
    echo -e "  Comandos utiles:"
    echo -e "    hsd-cli info                  Ver estado del nodo"
    echo -e "    hsd-cli rpc getblockcount     Ver altura de la cadena"
    echo -e "    hsd-cli rpc getpeerinfo       Ver nodos conectados"
    echo ""
    echo -e "  Para verificar resolucion DNS:"
    echo -e "    dig @127.0.0.1 -p ${LOCAL_DNS_PORT} portal.ierahkwa"
    echo ""
    echo -e "  La sincronizacion inicial de la cadena HNS puede tomar"
    echo -e "  entre 4 y 24 horas dependiendo de la conexion."
    echo ""
    echo -e "${GREEN}  Soberania digital: Tu DNS, tu control, tu nacion.${NC}"
    echo -e "${GREEN}============================================================${NC}"
}

# --- Ejecucion principal ---

main() {
    echo ""
    echo -e "${BLUE}============================================================${NC}"
    echo -e "${BLUE} IERAHKWA SOVEREIGN DNS - Instalacion de Handshake (HNS)${NC}"
    echo -e "${BLUE}============================================================${NC}"
    echo ""

    check_root
    detect_os
    install_system_dependencies
    setup_system_user
    install_hsd
    configure_hsd
    configure_local_dns
    create_systemd_service
    configure_matrix_link
    start_and_verify
    print_summary
}

main "$@"
