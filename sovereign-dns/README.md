# Ierahkwa Sovereign DNS - Handshake (HNS)

## Por que DNS soberano es necesario

El Sistema de Nombres de Dominio (DNS) convencional esta controlado por ICANN, una corporacion con sede en Estados Unidos. Esto significa que cualquier gobierno o entidad con suficiente influencia puede:

- Confiscar un dominio sin juicio previo
- Redirigir trafico a servidores de vigilancia
- Censurar sitios eliminando sus registros DNS
- Desconectar comunidades enteras del internet

Para la red Ierahkwa Ne Kanienke, esto es inaceptable. La soberania digital requiere control total sobre la capa de nombrado. Si no controlas tu DNS, no controlas tu presencia digital.

**Handshake (HNS)** resuelve este problema reemplazando la zona raiz del DNS con un blockchain descentralizado. Nadie puede confiscar el TLD `.ierahkwa` porque no existe una autoridad central que lo controle.

## Estructura de Archivos

```
sovereign-dns/
  setup_hns.sh              -- Script de instalacion y configuracion automatica
  handshake-config.json     -- Configuracion del nodo Handshake
  README.md                 -- Este archivo
```

## Que hace esta configuracion

1. **Instala un nodo completo de Handshake (hsd)** que participa en la red descentralizada de DNS
2. **Configura el resolver DNS local** para que todas las consultas a `.ierahkwa` se resuelvan a traves del nodo local, sin pasar por servidores DNS convencionales
3. **Registra zonas DNS internas** para los servicios de la red (Matrix, API, blockchain, votacion, Bio-Ledger)
4. **Crea un servicio del sistema** que mantiene el nodo ejecutandose permanentemente con reinicio automatico
5. **Enlaza con Matrix/Synapse** para que las comunicaciones cifradas de la red se resuelvan a traves del DNS soberano

## Requisitos

- **Sistema operativo**: Linux (Ubuntu 22.04+) o macOS
- **Node.js**: Version 18 o superior
- **Disco**: Al menos 50 GB libres para la cadena de bloques HNS
- **RAM**: 2 GB minimo para el nodo
- **Red**: Conexion a internet para la sincronizacion inicial de la cadena

## Instalacion

```bash
# Clonar o navegar al directorio sovereign-dns
cd /ruta/al/proyecto/sovereign-dns

# Hacer ejecutable el script
chmod +x setup_hns.sh

# Ejecutar como root (requiere permisos para configurar DNS y servicios)
sudo ./setup_hns.sh
```

El script ejecuta automaticamente:

1. Verificacion e instalacion de dependencias (Node.js, Git, herramientas de compilacion)
2. Creacion de usuario del sistema dedicado para seguridad
3. Clonado e instalacion de hsd desde el repositorio oficial de GitHub
4. Aplicacion de la configuracion desde `handshake-config.json`
5. Configuracion del resolver DNS local (systemd-resolved en Linux, /etc/resolver en macOS)
6. Creacion del servicio del sistema (systemd en Linux, launchd en macOS)
7. Configuracion de zonas DNS para servicios internos de Ierahkwa
8. Inicio del servicio y verificacion

## Verificacion

Despues de la instalacion, verificar que el nodo esta funcionando:

```bash
# Ver estado del nodo
hsd-cli info

# Ver altura de la cadena (sincronizacion)
hsd-cli rpc getblockcount

# Ver nodos conectados en la red
hsd-cli rpc getpeerinfo

# Probar resolucion DNS soberana
dig @127.0.0.1 -p 5350 portal.ierahkwa
```

La sincronizacion inicial de la cadena de bloques puede tomar entre 4 y 24 horas.

## Configuracion de Red

El archivo `handshake-config.json` define:

- **Puertos de red**: 12038 (P2P), 12037 (HTTP API), 5350 (DNS autoritativo), 5351 (DNS recursivo)
- **Limites de conexion**: 32 entrantes, 8 salientes
- **Indexacion**: Transacciones y direcciones indexadas para consultas rapidas
- **Seguridad**: Wallet API protegida, escuchando solo en localhost

## Servicios DNS Internos

La zona DNS configura los siguientes servicios:

| Dominio | IP | Servicio |
|---|---|---|
| portal.ierahkwa | 10.42.0.1 | Portal principal |
| api.ierahkwa | 10.42.0.2 | API de servicios |
| chain.ierahkwa | 10.42.0.3 | Blockchain MameyNode |
| identity.ierahkwa | 10.42.0.4 | Identidad soberana |
| bioledger.ierahkwa | 10.42.0.5 | Bio-Ledger ambiental |
| vote.ierahkwa | 10.42.0.6 | Votacion descentralizada |
| veritas.ierahkwa | 10.42.0.7 | Verificacion de hechos |
| emergency.ierahkwa | 10.42.0.8 | Comunicaciones de emergencia |
| code.ierahkwa | 10.42.0.9 | Repositorio de codigo |
| matrix.ierahkwa | 10.42.0.10 | Servidor Matrix/Synapse |

## Seguridad

- El nodo corre bajo un usuario del sistema dedicado sin shell de login
- Los directorios de datos tienen permisos restrictivos (750)
- El servicio systemd aplica protecciones adicionales: ProtectSystem, ProtectHome, NoNewPrivileges
- La wallet HTTP solo escucha en localhost
- Todas las conexiones P2P usan Brontide (cifrado de transporte basado en Noise Protocol)

## Integracion con la Red Ierahkwa

Este nodo DNS es un componente fundamental de la infraestructura soberana:

- Los nodos **MameyNode** usan DNS soberano para descubrirse mutuamente
- Los clientes **Matrix** resuelven el servidor de chat a traves de HNS
- Las plataformas web acceden a APIs a traves de dominios `.ierahkwa`
- El sistema de **identidad soberana** se verifica contra registros DNS controlados por la comunidad
- En modo offline, los nodos locales mantienen cache DNS para operacion sin conectividad

## Mantenimiento

```bash
# Ver logs del servicio (Linux)
journalctl -u ierahkwa-hsd -f

# Ver logs del servicio (macOS)
tail -f /var/log/ierahkwa/hsd/hsd-stdout.log

# Reiniciar el servicio (Linux)
sudo systemctl restart ierahkwa-hsd

# Actualizar hsd
cd /opt/ierahkwa/hsd
sudo git fetch --tags
sudo git checkout <nueva-version>
sudo npm install --production
sudo systemctl restart ierahkwa-hsd
```
