# LibreOffice headless — Conversión de documentos (opcional)

Ofimática libre (**LibreOffice**, Apache OpenOffice) para escritorio está en la lista de software libre recomendado. En servidor se puede usar **LibreOffice en modo headless** para convertir documentos (DOCX, ODS, ODT) a PDF sin depender de servicios externos.

---

## Cuándo usar

- **Ya tenemos:** Generación de PDF con **pdfkit** en Node (`node/services/pdf-generator.js`) para informes, certificados, estados de cuenta.
- **Opcional:** Si se necesitan conversiones **DOCX/ODT/ODS → PDF** desde el servidor (p. ej. plantillas gubernamentales en Writer/Calc), LibreOffice headless es la opción soberana (software libre, self-hosted).

---

## Instalación (Ubuntu/Debian)

```bash
# LibreOffice (incluye Writer, Calc, Impress; headless para servidor)
sudo apt update
sudo apt install -y libreoffice-writer libreoffice-calc libreoffice-impress libreoffice-headless
```

En entornos mínimos suele bastar:

```bash
sudo apt install -y libreoffice-writer libreoffice-headless
```

---

## Uso desde línea de comandos

```bash
# DOCX/ODT → PDF (ejemplo)
libreoffice --headless --convert-to pdf --outdir /ruta/salida /ruta/documento.docx
```

Desde Node se puede llamar con `child_process.exec` o `execFile`:

```javascript
const { execFile } = require('child_process');
const path = require('path');

function convertToPdf(inputPath, outputDir, callback) {
  execFile(
    'libreoffice',
    ['--headless', '--convert-to', 'pdf', '--outdir', outputDir, inputPath],
    { timeout: 60000 },
    (err, stdout, stderr) => {
      if (err) return callback(err);
      callback(null, path.join(outputDir, path.basename(inputPath, path.extname(inputPath)) + '.pdf'));
    }
  );
}
```

---

## Consideraciones

- **Rendimiento:** Cada conversión lanza un proceso LibreOffice; para muchas conversiones simultáneas valorar cola (Bull, etc.) o pool de workers.
- **Seguridad:** No ejecutar con documentos no confiables sin aislar (usuario limitado, contenedor).
- **Alternativa sin LibreOffice:** Seguir usando solo **pdfkit** para todo lo que se genere desde código (informes, certificados, estados de cuenta).

---

## Referencia

- Stack y ventajas/desventajas del software libre: `docs/SOFTWARE-LIBRE-STACK.md`.
- Principio todo propio: `PRINCIPIO-TODO-PROPIO.md`.
