# NEXUS Dashboard

## Descripcion

El **NEXUS Dashboard** es el centro de comando en tiempo real para los 18 mega-portales
NEXUS que componen la infraestructura digital soberana de **Ierahkwa Ne Kanienke**.

Proporciona visibilidad completa del estado operativo de las 422 plataformas,
los 7 agentes AI y los 109 tokens IGT que conforman la nacion digital soberana
para más de mil millones de personas en las Américas en 35+ países y 574 naciones tribales.

## Version

- **Plataforma**: nexus-dashboard v5.5.0
- **Ultima actualizacion**: 2026-03-01

## Caracteristicas

- **18 tarjetas NEXUS** con estado en vivo, conteo de plataformas, uptime y mini-graficos
- **Estadisticas globales**: 422 plataformas, 18 NEXUS, 7 agentes AI, 109 tokens IGT
- **Mapa interactivo** CSS que visualiza conexiones entre mega-portales
- **Monitor de salud** con puntuacion general, tiempos de actividad y datos del sistema
- **Feed de actividad** mostrando los ultimos despliegues y actualizaciones
- **Expansion por clic** para ver sub-plataformas de cada NEXUS
- Accesibilidad GAAD completa (skip-nav, aria, roles, reduced-motion)
- Tema oscuro (#0a0a0f) con acento #00FF41

## Arquitectura

```
nexus-dashboard/
  index.html       <- UI principal (~55KB, auto-contenida)
  README.md        <- Este archivo
  WHITEPAPER.md    <- Documento tecnico completo
  BLUEPRINT.md     <- Planos tecnicos y diagramas
```

### Dependencias

| Recurso                       | Tipo     | Proposito                     |
|-------------------------------|----------|-------------------------------|
| `../shared/ierahkwa.css`      | CSS      | Sistema de diseno compartido  |
| `../shared/ierahkwa-agents.js`| JS       | 7 agentes AI soberanos        |
| CSS/JS inline                 | Interno  | Logica y estilos del dashboard|

### Componentes Principales

1. **Header** - Logo, navegacion, indicador de estado en vivo, badge Quantum-Safe
2. **Hero** - Badge "NEXUS Command Center" con subtitulo descriptivo
3. **Stats Row** - 4 tarjetas con animacion de contadores
4. **NEXUS Grid** - 18 tarjetas expandibles en grid responsivo
5. **Mapa de Conexiones** - Visualizacion CSS radial con nodos interactivos
6. **Monitor de Salud** - Puntuacion, uptime por servicio, info del sistema
7. **Feed de Actividad** - 8 eventos recientes con iconos y timestamps
8. **Footer** - Links de navegacion al portal principal y repositorio

## Los 18 NEXUS

| #  | NEXUS           | Color   | Categoria         | Plataformas |
|----|-----------------|---------|-------------------|-------------|
| 1  | Orbital         | #00bcd4 | Telecom           | 21          |
| 2  | Escudo          | #f44336 | Defense            | 21          |
| 3  | Cerebro         | #7c4dff | AI/Quantum         | 23          |
| 4  | Tesoro          | #ffd600 | Finance            | 36          |
| 5  | Voces           | #e040fb | Social/Media       | 16          |
| 6  | Consejo         | #1565c0 | Government         | 27          |
| 7  | Tierra          | #43a047 | Nature             | 22          |
| 8  | Forja           | #00e676 | Dev Tools          | 101         |
| 9  | Urbe            | #ff9100 | Smart City         | 11          |
| 10 | Raices          | #00FF41 | Culture            | 16          |
| 11 | Salud           | #FF5722 | Health             | 9           |
| 12 | Academia        | #9C27B0 | University         | 5           |
| 13 | Escolar         | #1E88E5 | K-12 School        | 10          |
| 14 | Entretenimiento | #E91E63 | Entertainment      | 13          |
| 15 | Escritorio      | #26C6DA | Desktop Apps       | 17          |
| 16 | Comercio        | #FF6D00 | Commerce           | 17          |
| 17 | Amparo          | #607D8B | Social Protection  | 13          |
| 18 | Cosmos          | #1a237e | Space/Satellite    | 8           |

## Uso

Abrir `index.html` en cualquier navegador moderno. No requiere servidor ni
dependencias externas mas alla de los archivos compartidos del proyecto.

### Navegacion

- Las tarjetas NEXUS se expanden al hacer clic para mostrar sub-plataformas
- Los nodos del mapa enlazan a la tarjeta correspondiente en la cuadricula
- La barra de navegacion conecta con Admin Dashboard y Agentes AI

## Accesibilidad

- Skip navigation link para acceso directo al contenido
- Atributos `aria-label`, `role`, `aria-expanded` en todos los elementos interactivos
- Soporte completo de navegacion por teclado (Tab, Enter, Space)
- Media query `prefers-reduced-motion` para desactivar animaciones
- Contraste WCAG AA en toda la interfaz

## Licencia

Propiedad de Ierahkwa Ne Kanienke - Nacion Digital Soberana.
