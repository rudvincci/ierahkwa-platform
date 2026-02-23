# Ierahkwa Image Upload Service

Servicio de subida de imágenes para la Plataforma Ierahkwa con soporte de Drag & Drop, barras de progreso y vista previa.

## Características

| Característica | Descripción |
|----------------|-------------|
| **Múltiples archivos** | Sube múltiples imágenes simultáneamente |
| **Drag & Drop** | Arrastra y suelta con DropzoneJS v5.7.2 |
| **Barras de progreso** | Progreso en tiempo real por archivo |
| **Vista previa** | Previsualización antes y después de subir |
| **Galería** | Vista de galería con búsqueda y eliminación |
| **Miniaturas** | Generación automática de thumbnails |
| **Multiplataforma** | Windows, macOS, Linux |

## Tecnologías

- **Node.js + Express** - Backend
- **Multer** - Manejo de archivos
- **Sharp** - Procesamiento de imágenes (thumbnails)
- **DropzoneJS v5.7.2** - Drag & Drop frontend
- **Bootstrap v4.3.1** - UI Framework
- **jQuery v3.5.1** - JavaScript library

## Compatibilidad de Navegadores

| Navegador | Versión |
|-----------|---------|
| Google Chrome | 7+ |
| Apple Safari | 6+ |
| Mozilla Firefox | 4+ |
| Opera | 12+ |
| Microsoft Edge | Todos |
| Internet Explorer | 10+ |

## Instalación

```bash
cd image-upload
npm install
npm start
```

El servidor se iniciará en: **http://localhost:3500**

## Estructura del Proyecto

```
image-upload/
├── server.js           # Servidor Express principal
├── package.json        # Dependencias npm
├── public/
│   ├── index.html      # Frontend con Dropzone
│   └── uploads/        # Imágenes subidas
│       └── thumbnails/ # Miniaturas generadas
└── README.md
```

## API Endpoints

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/upload` | Subir archivo (Dropzone) |
| `POST` | `/api/upload/single` | Subir archivo individual |
| `POST` | `/api/upload/multiple` | Subir múltiples archivos |
| `DELETE` | `/api/upload/:fileName` | Eliminar archivo |
| `GET` | `/api/upload/list` | Listar archivos subidos |
| `GET` | `/api/stats` | Estadísticas de almacenamiento |

## Configuración

Editar las constantes en `server.js`:

```javascript
const CONFIG = {
    uploadDir: './public/uploads',
    thumbnailDir: './public/uploads/thumbnails',
    maxFileSize: 10 * 1024 * 1024, // 10MB
    allowedTypes: ['image/jpeg', 'image/png', 'image/gif', 'image/bmp', 'image/webp'],
    allowedExtensions: ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp']
};
```

## Modos de Subida

### 1. Dropzone (Recomendado)
- Arrastra y suelta múltiples archivos
- Vista previa automática
- Progreso individual por archivo
- Cola de procesamiento

### 2. Archivo Individual
- Selector de archivo tradicional
- Vista previa antes de subir
- Barra de progreso

### 3. Múltiples Archivos
- Selección múltiple
- Vista previa en cuadrícula
- Progreso general e individual

## Integración con Plataforma Ierahkwa

Este servicio está registrado en `platform-services.json`:

```json
{
  "id": "image-upload",
  "name": "Ierahkwa Image Upload",
  "domain": "images.ierahkwa.gov",
  "localPort": 3500,
  "token": "IGT-CLOUD"
}
```

## Licencia

MIT - Sovereign Government of Ierahkwa Ne Kanienke
