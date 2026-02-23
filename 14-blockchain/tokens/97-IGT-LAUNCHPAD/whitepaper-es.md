# IGT-LAUNCHPAD - Whitepaper

## Ierahkwa IDO Factory - Plataforma de Lanzamiento de Tokens

### Resumen Ejecutivo

IGT-LAUNCHPAD es el token oficial de la plataforma de lanzamiento de tokens del Gobierno Soberano de Ierahkwa Ne Kanienke. La plataforma IDO Factory permite crear pools de IDO (Initial DEX Offering), bloqueos de tokens y campañas de crowdsale en la Blockchain Soberana de Ierahkwa y todas las redes compatibles con EVM.

### Información del Token

| Atributo | Valor |
|----------|-------|
| Nombre | Ierahkwa Futurehead Launchpad |
| Símbolo | IGT-LAUNCHPAD |
| ID de Departamento | 97 |
| Red | Ierahkwa Sovereign Blockchain |
| Chain ID | 777777 |
| Decimales | 9 |
| Suministro Total | 10,000,000,000,000 |

### Características de la Plataforma

#### 1. Creación de Pools IDO
- **Pools Públicos**: Abiertos a todos los participantes
- **Pools Whitelist**: Solo para direcciones autorizadas
- **Pools Escalonados**: Diferentes niveles de participación
- **Pools Lotería**: Sistema aleatorio de asignación

#### 2. Token Locker
- Bloqueo de tokens del equipo
- Bloqueo de liquidez
- Vesting lineal configurable
- Períodos de cliff
- Retiro de emergencia con penalización

#### 3. Configuración Administrativa
- Logo y branding personalizables
- Colores de la plataforma
- Enlaces sociales
- Direcciones de administradores
- Configuración de tarifas

#### 4. Multi-Chain
Redes soportadas:
- Ierahkwa Sovereign Blockchain (777777)
- Ethereum Mainnet (1)
- BNB Smart Chain (56)
- Polygon (137)
- Avalanche C-Chain (43114)
- Arbitrum One (42161)
- Fantom Opera (250)

### Tarifas

| Concepto | Tarifa |
|----------|--------|
| Creación de Pool | 0.1 token nativo |
| Tarifa de Plataforma | 3% de fondos recaudados |
| Bloqueo de Tokens | 0.05 token nativo |
| Retiro de Emergencia | 10% penalización |

### Tecnología

- **Backend**: .NET 10
- **API**: RESTful con Swagger/OpenAPI
- **Frontend**: HTML5, CSS3, JavaScript
- **Smart Contracts**: Solidity (EVM compatible)

### API Endpoints

```
GET  /api/ido/pools            - Obtener todos los pools
GET  /api/ido/pools/active     - Pools activos
POST /api/ido/pools            - Crear pool
POST /api/ido/pools/{id}/contribute - Contribuir
GET  /api/locker               - Obtener bloqueos
POST /api/locker               - Crear bloqueo
GET  /api/admin/config         - Configuración
```

### Infraestructura

- **Nodo**: Ierahkwa Futurehead Mamey Node
- **Banco Central**: Ierahkwa Futurehead BDET Bank
- **Consenso**: Sovereign Proof of Authority (SPoA)
- **Estándar**: Ierahkwa Government Token (IGT)

### Contacto

- **Plataforma**: https://launchpad.ierahkwa.gov
- **Local**: http://localhost:5097
- **Swagger**: http://localhost:5097/api

---

© 2026 Sovereign Government of Ierahkwa Ne Kanienke
Office of the Prime Minister
All Rights Reserved
