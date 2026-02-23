# Modelos de machine learning — Referencia

**Sovereign Government of Ierahkwa Ne Kanienke** · Ciencia de datos y ML  
Referencia: por qué usar ML, tipos de modelos (supervisado / no supervisado), regresión, clasificación, árboles, clustering, métricas y uso con Scikit-Learn (todo propio, sin dependencias de terceros).

---

## Por qué utilizar modelos de ML

Las organizaciones usan **modelización predictiva** para maximizar ingresos y crecimiento. Ejemplos:

- **Sistemas de recomendación** (Netflix, Spotify): ML no supervisado para recomendar contenido según actividad del usuario; mejor experiencia → más retención.
- **Análisis de opinión y oferta** (operadores móviles): ML supervisado para adaptar productos a la demanda.

**Clasificación principal:** **supervisado** (entrada y salida etiquetadas) vs **no supervisado** (datos sin etiquetas).

---

## ¿Qué es el machine learning supervisado?

El **aprendizaje supervisado** aprende la **relación entre entrada (X) y salida (Y)** a partir de **datos etiquetados**: cada ejemplo tiene características (X) y objetivo (Y). Ese proceso se llama **entrenamiento** o **ajuste**. Hay dos tipos:

- **Clasificación:** predecir un resultado **discreto** (ej. sí/no, impago/no impago, fraude/no fraude). Si solo hay dos clases → clasificación **binaria**; más de dos → **multiclase**. Algoritmos típicos: regresión logística, árbol de decisión, KNN, bosque aleatorio, redes neuronales.
- **Regresión:** predecir un valor **continuo** (ventas, salario, precio de vivienda, temperatura). Algoritmos típicos: regresión lineal, árbol de decisión, KNN, bosque aleatorio, redes neuronales.

Ejemplo clasificación: banco con historial crediticio, préstamos, inversiones (X) y si el cliente impagó (Y = 1/0). Ejemplo regresión: tamaño, dormitorios, barrio (X) y precio de la casa (Y).

---

## Supervisado vs no supervisado vs semisupervisado

| Aspecto | Supervisado | No supervisado | Semisupervisado |
|---------|-------------|----------------|------------------|
| **Datos** | Etiquetados (X e Y). El algoritmo aprende a predecir Y dado X. | Sin etiquetas; el algoritmo descubre estructura. | Poco dato etiquetado + mucho sin etiqueta. |
| **Casos de uso** | Clasificación, regresión. | Clustering, detección de anomalías. | Cuando las etiquetas son caras o escasas. |
| **Objetivo** | Predecir Y en datos nuevos a partir del entrenamiento con (X, Y). | Extraer información sin etiquetas explícitas. | Aprovechar muchos datos no etiquetados para mejorar el modelo. |
| **Complejidad** | Más sencillo de evaluar (tenemos Y). | Suele requerir más datos y validación humana de la salida. | Intermedio; tipos: transductivo e inductivo. |

Los modelos supervisados suelen dar resultados más precisos cuando las etiquetas son correctas; si las etiquetas están mal, aprenden errores. Los no supervisados no usan Y en el entrenamiento; la validación de los resultados (p. ej. si un clúster tiene sentido) sigue siendo humana.

---

## Supervisado: regresión vs clasificación

| Tipo | Objetivo | Variable dependiente (y) |
|------|----------|---------------------------|
| **Regresión** | Predecir un valor **continuo** | Numérica (ej. precio alquiler, ventas) |
| **Clasificación** | Predecir una **clase o categoría** | Discreta (ej. Sí/No, spam/no spam, especie) |

---

## Modelos de regresión

### Métricas (no usar precisión; esa es para clasificación)

| Métrica | Fórmula / idea | Uso |
|---------|----------------|-----|
| **MAE** | Media de \|y_real − y_pred\| | Error en mismas unidades que y |
| **MSE** | Media de (y_real − y_pred)² | Penaliza más los errores grandes |
| **RMSE** | √MSE | Mismas unidades que y; interpretable |

### Tipos

1. **Regresión lineal simple**  
   Relación lineal y = mx + c; “línea de mejor ajuste” por **mínimos cuadrados** (minimizar suma de errores al cuadrado).  
   **Pros:** sencillo, interpretable, funciona bien con relaciones lineales. **Contras:** asume linealidad; fácil de sobreajustar.

2. **Ridge**  
   Igual que lineal pero se añade **penalización** λ · Σ(coef²) a la función de coste. Coeficientes se reducen pero no llegan a cero.  
   - λ = 0 → como lineal. λ alto → modelo más simple; λ demasiado alto → **subajuste**.

3. **Lasso**  
   Penalización λ · Σ\|coef\|. Los coeficientes **pueden ser cero** → sirve para **selección de variables** (quitar las que no aportan).

### Scikit-Learn (Python, código abierto)

```python
from sklearn.linear_model import LinearRegression, Ridge, Lasso

lr_model = LinearRegression()
lr_model.fit(X_train, y_train)

model_ridge = Ridge(alpha=1.0)
model_ridge.fit(X_train, y_train)

model_lasso = Lasso(alpha=1.0)
model_lasso.fit(X_train, y_train)
```

---

## Modelos de clasificación

### Métricas

| Métrica | Idea |
|---------|------|
| **Exactitud (accuracy)** | Fracción de predicciones correctas. No basta sola si las clases están desbalanceadas. |
| **Precisión** | Calidad de las predicciones **positivas** (TP / (TP + FP)). |
| **Exhaustividad (recall)** | Calidad de las predicciones **negativas** / sensibilidad (TP / (TP + FN)). |
| **F1** | Media armónica de precisión y exhaustividad. |
| **AUC** | Capacidad del modelo para distinguir entre clases. |

Ejemplo: enfermedad rara (5 % positivos). Un modelo que predice “todos negativos” tiene 95 % exactitud pero es inútil; hay que mirar precisión y recall. En salud suele priorizarse **recall** (no perder casos positivos). En bloqueo de usuarios maliciosos puede priorizarse **precisión** (evitar bloquear legítimos).

### Tipos

1. **Regresión logística**  
   Predice **probabilidad** (entre 0 y 1) con una curva en S (función logística). Umbral por defecto 0,5 para clasificar en dos clases. Coeficientes por **máxima verosimilitud**, no mínimos cuadrados.  
   **Pros:** sencillo, interpretable, salidas bien calibradas como probabilidad. **Contras:** sobreajuste; limitado para relaciones muy complejas.

2. **K-Nearest Neighbors (KNN)**  
   Clasifica según la **clase mayoritaria** entre los k vecinos más cercanos.  
   - k pequeño → más ruido y sensibilidad a outliers.  
   - k grande → puede diluir clases minoritarias.  
   **Pros:** no asume forma de los datos; intuitivo. **Contras:** entrenamiento costoso en tiempo; con muchas características pierde precisión.

3. **Ingenuo Bayes (Naive Bayes)**  
   Usa el **teorema de Bayes** para estimar probabilidades condicionadas; asigna la **clase con mayor probabilidad**. Asume independencia entre características (por eso "ingenuo").  
   **Pros:** muy rápido y fácil de implementar. **Contras:** asunción de independencia a menudo falsa; "problema de frecuencia cero" si una categoría no aparece en entrenamiento (probabilidad 0).

### Scikit-Learn

```python
from sklearn.linear_model import LogisticRegression
from sklearn.neighbors import KNeighborsClassifier
from sklearn.naive_bayes import GaussianNB

log_reg = LogisticRegression()
log_reg.fit(X_train, y_train)

knn = KNeighborsClassifier()
knn.fit(X_train, y_train)

nb = GaussianNB()
nb.fit(X_train, y_train)
```

---

## Modelos basados en árboles

Sirven para **clasificación y regresión**.

1. **Árbol de decisión**  
   Divisiones sucesivas según atributos (p. ej. entropía, ganancia de información) hasta llegar a una hoja (decisión).  
   **Pros:** muy intuitivo y fácil de explicar; poca preparación de datos. **Contras:** inestable (pequeños cambios en datos pueden cambiar mucho el árbol); propenso a **sobreajuste**.

2. **Bosque aleatorio (Random Forest)**  
   Muchos árboles de decisión entrenados con **muestreo aleatorio** de filas y/o variables; la predicción final es **voto mayoritario** (clasificación) o **media** (regresión).  
   **Pros:** maneja relaciones no lineales; selección implícita de características; generaliza mejor. **Contras:** menos interpretable; costoso con conjuntos de datos muy grandes.

### Scikit-Learn

```python
from sklearn.tree import DecisionTreeClassifier, DecisionTreeRegressor
from sklearn.ensemble import RandomForestClassifier, RandomForestRegressor

clf = DecisionTreeClassifier()
dt_reg = DecisionTreeRegressor()

rf_clf = RandomForestClassifier()
rf_reg = RandomForestRegressor()
```

---

## Clustering (no supervisado)

**Objetivo:** agrupar objetos **similares** entre sí y **distintos** de otros. Usos: recomendación por patrones similares, detección de anomalías, segmentación de clientes.

### K-Means

1. Elegir **k** (número de clusters).  
2. Inicializar centroides (p. ej. aleatorio o k-means++).  
3. Asignar cada punto al centroide más cercano.  
4. Recalcular centroides y repetir hasta convergencia.

**Cómo elegir k:** p. ej. método del codo (gráfica de error vs k; elegir el “codo”).

```python
from sklearn.cluster import KMeans
kmeans = KMeans(n_clusters=3, init='k-means++')
kmeans.fit(X)
```

---

## Resumen rápido: qué modelo usar

| Objetivo | Modelo sugerido |
|----------|-----------------|
| Predicción numérica (regresión), interpretable | Regresión lineal, Ridge, Lasso |
| Clasificación binaria o multiclase, probabilidades | Regresión logística |
| Clasificación por “vecinos” | KNN |
| Interpretabilidad y reglas claras | Árbol de decisión |
| Mejor generalización (clasificación/regresión) | Bosque aleatorio |
| Agrupar sin etiquetas | K-Means (u otros clustering) |
| Clasificación rápida, texto/categorías | Ingenuo Bayes |

Un “buen” modelo depende del **caso de uso** y de las **métricas** relevantes (en clasificación no basta con la exactitud; revisar precisión, recall, F1, AUC). Temas no cubiertos: evaluación de modelos, validación cruzada, ajuste de hiperparámetros (ver documentación de scikit-learn).

---

## Ejemplo de código Python (clasificación binaria)

Pipeline mínimo con Scikit-Learn: datos sintéticos, partición train/test, regresión logística y exactitud.

```python
from sklearn.datasets import make_classification
from sklearn.model_selection import train_test_split
from sklearn.linear_model import LogisticRegression
from sklearn.metrics import accuracy_score

# Dataset sintético: 1000 muestras, 10 características, 2 clases
X, y = make_classification(n_samples=1000, n_features=10, n_classes=2)

# Partición 75 % train, 25 % test
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.25, random_state=42)

# Modelo y entrenamiento
lr = LogisticRegression()
lr.fit(X_train, y_train)

# Predicciones y exactitud
y_pred = lr.predict(X_test)
accuracy_score(y_test, y_pred)
```

En producción conviene evaluar también precisión, recall, F1 y (si aplica) AUC.

---

## En Ierahkwa (todo propio)

- **Librerías:** Scikit-Learn es código abierto y se usa en nuestro stack sin APIs de terceros (ver `docs/MARCOS-Y-BIBLIOTECAS-IA-REFERENCIA.md`).
- **Datos y entrenamiento:** modelos entrenados con datos propios; sin enviar datos a servicios externos.
- **Integración con IA generativa:** para generación de código o sugerencias de pipelines ML se usa Ollama/ai-soberano (ver `docs/MODELOS-LLM-COMPARACION.md`, `docs/PROYECTOS-IA-GENERATIVA-PORTAFOLIO.md`).

Referencias: `PRINCIPIO-TODO-PROPIO.md`, `MODELOS-GENERATIVOS-EXPLICACION.md`, `INDEX-DOCUMENTACION.md`.
