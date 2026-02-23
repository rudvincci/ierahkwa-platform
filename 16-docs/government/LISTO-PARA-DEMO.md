# Dejar todo listo para el demo

**Sovereign Government of Ierahkwa Ne Kanienke**  
Para que **ningún botón falle** y no pases pena en el demo.

---

## 1. Arrancar el Node (obligatorio)

Los botones y las pantallas llaman a la API en el puerto **8545**. Si el Node no está corriendo, las llamadas fallan y nada responde.

```bash
./start.sh
```

O solo lo mínimo para probar:

```bash
cd RuddieSolution/node && NODE_ENV=production node server.js
```

Comprueba que responde: **http://localhost:8545/health**

---

## 2. Abrir las plataformas desde 8545

Para que los botones y la API funcionen, las páginas tienen que cargarse desde **http://localhost:8545** (el mismo origen que la API).

- **Bien:** http://localhost:8545/platform/index.html  
- **Mal:** http://localhost:8080/index.html (el 8080 no tiene API; los fetch fallan)

El script que abre todo en Chrome ya usa 8545:

```bash
./scripts/abrir-todas-plataformas-chrome.sh
```

Así todas las pestañas abren con base en 8545 y la API responde.

---

## 3. Endpoints que ya existen (para que no fallen los botones)

El cliente de la plataforma llama a:

- `/api/config` — config.json  
- `/api/platform/overview` — resumen (links, departments)  
- `/api/platform/services` — servicios  
- `/api/platform/health` — salud  
- `/api/platform/links` — enlaces  
- `/api/platform/departments` — departamentos  
- `/api/platform/modules` — módulos  
- `/api/platform/tokens` — tokens  

Todos están implementados en el Node. Si algo no responde, revisa que el servidor esté arriba y que abras desde **8545**.

---

## 4. Resumen “listo para demo”

1. **./start.sh** (Node + servicios en marcha).  
2. **./scripts/abrir-todas-plataformas-chrome.sh** (abrir todo desde 8545 en Chrome).  
3. En localhost el **security-layer** te deja entrar como leader sin login.  
4. Si un botón no hace nada: F12 → pestaña Red/Network y mira si la petición va a 8545 y si devuelve 200 o error.

Con esto deberías tener **todo listo para el demo** y que los botones respondan.
