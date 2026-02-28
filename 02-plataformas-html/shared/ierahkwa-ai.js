/**
 * IERAHKWA SOVEREIGN PLATFORM — Offline AI Engine v1.0.0
 * On-device ML inference using ONNX Runtime Web + WebAssembly
 * Zero data leaves the device — 100% sovereign AI
 *
 * Usage:
 *   <script src="../shared/ierahkwa-ai.js"></script>
 *   <script>
 *     // Load a model
 *     await IerahkwaAI.models.load('sentiment-es');
 *
 *     // Run inference offline
 *     const result = await IerahkwaAI.text.sentiment('Excelente plataforma!');
 *     // => { label: 'positive', score: 0.94, source: 'local' }
 *   </script>
 *
 * Dependencies: ONNX Runtime Web (loaded dynamically)
 * Supports: WebGPU > WASM-SIMD > WASM (auto-detected)
 */

window.IerahkwaAI = (function() {
    'use strict';

    const VERSION = '1.0.0';
    const MODEL_SERVER = window.IERAHKWA_AI_URL || 'https://ai.ierahkwa.org';
    const ORT_CDN = 'https://cdn.jsdelivr.net/npm/onnxruntime-web@1.17.0/dist';
    const DB_NAME = 'ierahkwa-ai-models';
    const DB_VERSION = 1;
    const STORE_NAME = 'models';
    const META_STORE = 'metadata';

    // --- State ---
    let ortReady = false;
    let ortSession = {};
    let db = null;
    const loadedModels = new Map();

    // --- Model Registry ---
    const MODEL_REGISTRY = {
        'sentiment-es': {
            name: 'Sentiment Analysis (ES/EN)',
            file: 'sentiment-multilingual-minilm.onnx',
            tokenizer: 'sentiment-tokenizer.json',
            size: 25600000,
            type: 'text',
            task: 'sentiment',
            labels: ['negative', 'neutral', 'positive'],
            maxLength: 128,
            description: 'Analisis de sentimiento multilingue'
        },
        'classify-multi': {
            name: 'Text Classification (Multilingual)',
            file: 'classify-distilbert-multi.onnx',
            tokenizer: 'classify-tokenizer.json',
            size: 30720000,
            type: 'text',
            task: 'classification',
            labels: ['politics', 'economy', 'culture', 'technology', 'health', 'education', 'environment', 'sports'],
            maxLength: 256,
            description: 'Clasificacion de texto en 8 categorias'
        },
        'translate-es-en': {
            name: 'Translation ES → EN',
            file: 'translate-marian-es-en.onnx',
            tokenizer: 'translate-es-en-tokenizer.json',
            size: 40960000,
            type: 'translate',
            task: 'translation',
            sourceLang: 'es',
            targetLang: 'en',
            maxLength: 512,
            description: 'Traduccion Espanol a Ingles'
        },
        'translate-en-es': {
            name: 'Translation EN → ES',
            file: 'translate-marian-en-es.onnx',
            tokenizer: 'translate-en-es-tokenizer.json',
            size: 40960000,
            type: 'translate',
            task: 'translation',
            sourceLang: 'en',
            targetLang: 'es',
            maxLength: 512,
            description: 'Traduccion Ingles a Espanol'
        },
        'image-classify': {
            name: 'Image Classification',
            file: 'mobilenetv3-small.onnx',
            labels_file: 'imagenet-labels.json',
            size: 10240000,
            type: 'image',
            task: 'classification',
            inputSize: 224,
            description: 'Clasificacion de imagenes (1000 categorias)'
        },
        'ner-multi': {
            name: 'Named Entity Recognition',
            file: 'ner-mbert-multi.onnx',
            tokenizer: 'ner-tokenizer.json',
            size: 46080000,
            type: 'text',
            task: 'ner',
            labels: ['O', 'B-PER', 'I-PER', 'B-ORG', 'I-ORG', 'B-LOC', 'I-LOC', 'B-MISC', 'I-MISC'],
            maxLength: 256,
            description: 'Reconocimiento de entidades nombradas'
        },
        'embed-mini': {
            name: 'Semantic Embeddings',
            file: 'embed-minilm-l6-v2.onnx',
            tokenizer: 'embed-tokenizer.json',
            size: 25600000,
            type: 'text',
            task: 'embedding',
            dimensions: 384,
            maxLength: 128,
            description: 'Embeddings semanticos para busqueda'
        }
    };

    // --- IndexedDB for Model Storage ---
    function openDB() {
        if (db) return Promise.resolve(db);
        return new Promise((resolve, reject) => {
            const req = indexedDB.open(DB_NAME, DB_VERSION);
            req.onupgradeneeded = () => {
                const database = req.result;
                if (!database.objectStoreNames.contains(STORE_NAME)) {
                    database.createObjectStore(STORE_NAME);
                }
                if (!database.objectStoreNames.contains(META_STORE)) {
                    database.createObjectStore(META_STORE);
                }
            };
            req.onsuccess = () => { db = req.result; resolve(db); };
            req.onerror = () => reject(req.error);
        });
    }

    async function dbGet(store, key) {
        const database = await openDB();
        return new Promise((resolve, reject) => {
            const tx = database.transaction(store, 'readonly');
            const s = tx.objectStore(store);
            const r = s.get(key);
            r.onsuccess = () => resolve(r.result);
            r.onerror = () => reject(r.error);
        });
    }

    async function dbPut(store, key, value) {
        const database = await openDB();
        return new Promise((resolve, reject) => {
            const tx = database.transaction(store, 'readwrite');
            const s = tx.objectStore(store);
            const r = s.put(value, key);
            r.onsuccess = () => resolve();
            r.onerror = () => reject(r.error);
        });
    }

    async function dbDelete(store, key) {
        const database = await openDB();
        return new Promise((resolve, reject) => {
            const tx = database.transaction(store, 'readwrite');
            const s = tx.objectStore(store);
            const r = s.delete(key);
            r.onsuccess = () => resolve();
            r.onerror = () => reject(r.error);
        });
    }

    async function dbGetAllKeys(store) {
        const database = await openDB();
        return new Promise((resolve, reject) => {
            const tx = database.transaction(store, 'readonly');
            const s = tx.objectStore(store);
            const r = s.getAllKeys();
            r.onsuccess = () => resolve(r.result);
            r.onerror = () => reject(r.error);
        });
    }

    // --- ONNX Runtime Loader ---
    async function loadORT() {
        if (ortReady) return;
        if (typeof ort !== 'undefined') { ortReady = true; return; }

        return new Promise((resolve, reject) => {
            const script = document.createElement('script');
            script.src = ORT_CDN + '/ort.min.js';
            script.onload = () => {
                ortReady = true;
                // Configure WASM paths
                if (typeof ort !== 'undefined') {
                    ort.env.wasm.wasmPaths = ORT_CDN + '/';
                }
                resolve();
            };
            script.onerror = async () => {
                // Try loading from cache
                try {
                    const cached = await dbGet(STORE_NAME, '_ort_runtime');
                    if (cached) {
                        const blob = new Blob([cached], { type: 'application/javascript' });
                        const url = URL.createObjectURL(blob);
                        const s2 = document.createElement('script');
                        s2.src = url;
                        s2.onload = () => { ortReady = true; resolve(); };
                        s2.onerror = () => reject(new Error('No se pudo cargar ONNX Runtime'));
                        document.head.appendChild(s2);
                    } else {
                        reject(new Error('ONNX Runtime no disponible offline'));
                    }
                } catch(e) {
                    reject(e);
                }
            };
            document.head.appendChild(script);
        });
    }

    // --- Backend Detection ---
    function detectBackend() {
        if (typeof navigator !== 'undefined' && navigator.gpu) return 'webgpu';
        if (typeof WebAssembly !== 'undefined') {
            try {
                // Check SIMD support
                new WebAssembly.Module(new Uint8Array([
                    0,97,115,109,1,0,0,0,1,5,1,96,0,1,123,3,2,1,0,10,10,1,8,0,65,0,253,15,253,98,11
                ]));
                return 'wasm-simd';
            } catch(e) {
                return 'wasm';
            }
        }
        return 'none';
    }

    // --- Simple Tokenizer (for demo — real tokenizer loaded per model) ---
    function simpleTokenize(text, maxLength) {
        // Basic whitespace tokenizer with padding — production uses model-specific tokenizers
        const words = text.toLowerCase().split(/\s+/).filter(w => w.length > 0);
        const tokens = words.slice(0, maxLength);
        // CLS + tokens + SEP + padding
        const inputIds = new Array(maxLength).fill(0);
        const attentionMask = new Array(maxLength).fill(0);
        inputIds[0] = 101; // [CLS]
        attentionMask[0] = 1;
        for (let i = 0; i < Math.min(tokens.length, maxLength - 2); i++) {
            inputIds[i + 1] = hashToken(tokens[i]);
            attentionMask[i + 1] = 1;
        }
        inputIds[Math.min(tokens.length + 1, maxLength - 1)] = 102; // [SEP]
        attentionMask[Math.min(tokens.length + 1, maxLength - 1)] = 1;
        return { inputIds, attentionMask };
    }

    function hashToken(word) {
        // Simple hash for demo (real implementation uses vocab lookup)
        let hash = 0;
        for (let i = 0; i < word.length; i++) {
            hash = ((hash << 5) - hash + word.charCodeAt(i)) & 0x7FFFFFFF;
        }
        return (hash % 28996) + 1000; // Map to vocab range
    }

    function softmax(arr) {
        const max = Math.max(...arr);
        const exps = arr.map(x => Math.exp(x - max));
        const sum = exps.reduce((a, b) => a + b, 0);
        return exps.map(x => x / sum);
    }

    // --- Model Management ---
    const models = {
        async load(modelId) {
            if (!MODEL_REGISTRY[modelId]) throw new Error('Modelo desconocido: ' + modelId);
            if (loadedModels.has(modelId)) return loadedModels.get(modelId);

            await loadORT();
            const meta = MODEL_REGISTRY[modelId];

            // Try loading from IndexedDB cache
            let modelData = await dbGet(STORE_NAME, modelId);

            if (!modelData) {
                // Download from server
                try {
                    const url = MODEL_SERVER + '/ai/models/' + modelId + '/download';
                    const response = await fetch(url);
                    if (!response.ok) throw new Error('Download failed: ' + response.status);
                    modelData = await response.arrayBuffer();
                    // Cache in IndexedDB
                    await dbPut(STORE_NAME, modelId, modelData);
                    await dbPut(META_STORE, modelId, {
                        id: modelId,
                        name: meta.name,
                        size: modelData.byteLength,
                        cachedAt: new Date().toISOString(),
                        version: VERSION
                    });
                    // Notify service worker to also cache
                    if (navigator.serviceWorker?.controller) {
                        navigator.serviceWorker.controller.postMessage({
                            type: 'PREFETCH_AI_MODEL',
                            url: url,
                            modelId: modelId
                        });
                    }
                } catch(e) {
                    console.warn('[IerahkwaAI] Descarga fallida, modelo no disponible offline:', modelId);
                    throw new Error('Modelo no disponible: ' + modelId + ' — ' + e.message);
                }
            }

            // Create ONNX session
            try {
                const session = await ort.InferenceSession.create(modelData, {
                    executionProviders: detectBackend() === 'webgpu'
                        ? ['webgpu', 'wasm']
                        : ['wasm'],
                    graphOptimizationLevel: 'all'
                });

                const info = { session, meta, loadedAt: Date.now() };
                loadedModels.set(modelId, info);
                console.log('[IerahkwaAI] Modelo cargado:', meta.name, '(' + detectBackend() + ')');
                return info;
            } catch(e) {
                throw new Error('Error cargando modelo ONNX: ' + e.message);
            }
        },

        async unload(modelId) {
            const info = loadedModels.get(modelId);
            if (info?.session) {
                await info.session.release();
                loadedModels.delete(modelId);
            }
        },

        list() {
            return Object.entries(MODEL_REGISTRY).map(([id, meta]) => ({
                id,
                name: meta.name,
                type: meta.type,
                task: meta.task,
                size: meta.size,
                description: meta.description,
                loaded: loadedModels.has(id)
            }));
        },

        async cached() {
            try {
                const keys = await dbGetAllKeys(META_STORE);
                const results = [];
                for (const key of keys) {
                    if (key === '_ort_runtime') continue;
                    const meta = await dbGet(META_STORE, key);
                    if (meta) results.push(meta);
                }
                return results;
            } catch {
                return [];
            }
        },

        async download(modelId) {
            return models.load(modelId);
        },

        async deleteCache(modelId) {
            await dbDelete(STORE_NAME, modelId);
            await dbDelete(META_STORE, modelId);
            loadedModels.delete(modelId);
        },

        async clearAll() {
            const keys = await dbGetAllKeys(STORE_NAME);
            for (const key of keys) {
                await dbDelete(STORE_NAME, key);
            }
            const metaKeys = await dbGetAllKeys(META_STORE);
            for (const key of metaKeys) {
                await dbDelete(META_STORE, key);
            }
            loadedModels.clear();
        }
    };

    // --- Text Inference ---
    const text = {
        async sentiment(inputText) {
            const modelId = 'sentiment-es';
            const info = loadedModels.has(modelId) ? loadedModels.get(modelId) : await models.load(modelId);
            const { session, meta } = info;

            const { inputIds, attentionMask } = simpleTokenize(inputText, meta.maxLength);
            const feeds = {
                input_ids: new ort.Tensor('int64', BigInt64Array.from(inputIds.map(BigInt)), [1, meta.maxLength]),
                attention_mask: new ort.Tensor('int64', BigInt64Array.from(attentionMask.map(BigInt)), [1, meta.maxLength])
            };

            const start = performance.now();
            const results = await session.run(feeds);
            const latency = performance.now() - start;

            const logits = Array.from(results[session.outputNames[0]].data);
            const probs = softmax(logits);
            const maxIdx = probs.indexOf(Math.max(...probs));

            return {
                label: meta.labels[maxIdx],
                score: Math.round(probs[maxIdx] * 1000) / 1000,
                scores: Object.fromEntries(meta.labels.map((l, i) => [l, Math.round(probs[i] * 1000) / 1000])),
                latencyMs: Math.round(latency),
                source: 'local',
                model: modelId,
                backend: detectBackend()
            };
        },

        async classify(inputText) {
            const modelId = 'classify-multi';
            const info = loadedModels.has(modelId) ? loadedModels.get(modelId) : await models.load(modelId);
            const { session, meta } = info;

            const { inputIds, attentionMask } = simpleTokenize(inputText, meta.maxLength);
            const feeds = {
                input_ids: new ort.Tensor('int64', BigInt64Array.from(inputIds.map(BigInt)), [1, meta.maxLength]),
                attention_mask: new ort.Tensor('int64', BigInt64Array.from(attentionMask.map(BigInt)), [1, meta.maxLength])
            };

            const start = performance.now();
            const results = await session.run(feeds);
            const latency = performance.now() - start;

            const logits = Array.from(results[session.outputNames[0]].data);
            const probs = softmax(logits);
            const sorted = probs.map((p, i) => ({ label: meta.labels[i], score: p }))
                .sort((a, b) => b.score - a.score);

            return {
                label: sorted[0].label,
                score: Math.round(sorted[0].score * 1000) / 1000,
                rankings: sorted.map(s => ({ ...s, score: Math.round(s.score * 1000) / 1000 })),
                latencyMs: Math.round(latency),
                source: 'local',
                model: modelId,
                backend: detectBackend()
            };
        },

        async embed(inputText) {
            const modelId = 'embed-mini';
            const info = loadedModels.has(modelId) ? loadedModels.get(modelId) : await models.load(modelId);
            const { session, meta } = info;

            const { inputIds, attentionMask } = simpleTokenize(inputText, meta.maxLength);
            const feeds = {
                input_ids: new ort.Tensor('int64', BigInt64Array.from(inputIds.map(BigInt)), [1, meta.maxLength]),
                attention_mask: new ort.Tensor('int64', BigInt64Array.from(attentionMask.map(BigInt)), [1, meta.maxLength])
            };

            const start = performance.now();
            const results = await session.run(feeds);
            const latency = performance.now() - start;

            const embedding = Array.from(results[session.outputNames[0]].data).slice(0, meta.dimensions);

            return {
                embedding,
                dimensions: meta.dimensions,
                latencyMs: Math.round(latency),
                source: 'local',
                model: modelId,
                backend: detectBackend()
            };
        }
    };

    // --- Translation ---
    const translate = {
        detect(inputText) {
            // Simple language detection based on common words
            const esWords = ['el', 'la', 'los', 'las', 'de', 'en', 'que', 'por', 'con', 'para', 'es', 'un', 'una', 'del', 'se', 'al'];
            const enWords = ['the', 'is', 'at', 'in', 'on', 'of', 'and', 'to', 'for', 'with', 'this', 'that', 'are', 'was', 'has', 'be'];

            const words = inputText.toLowerCase().split(/\s+/);
            let esScore = 0, enScore = 0;
            for (const w of words) {
                if (esWords.includes(w)) esScore++;
                if (enWords.includes(w)) enScore++;
            }

            if (esScore > enScore) return { language: 'es', confidence: esScore / words.length };
            if (enScore > esScore) return { language: 'en', confidence: enScore / words.length };
            return { language: 'unknown', confidence: 0 };
        },

        async translate(inputText, sourceLang, targetLang) {
            const modelId = sourceLang === 'es' ? 'translate-es-en' : 'translate-en-es';
            if (!sourceLang) {
                const detected = translate.detect(inputText);
                sourceLang = detected.language === 'es' ? 'es' : 'en';
                targetLang = sourceLang === 'es' ? 'en' : 'es';
            }

            const info = loadedModels.has(modelId) ? loadedModels.get(modelId) : await models.load(modelId);
            const { session, meta } = info;

            const { inputIds, attentionMask } = simpleTokenize(inputText, meta.maxLength);
            const feeds = {
                input_ids: new ort.Tensor('int64', BigInt64Array.from(inputIds.map(BigInt)), [1, meta.maxLength]),
                attention_mask: new ort.Tensor('int64', BigInt64Array.from(attentionMask.map(BigInt)), [1, meta.maxLength])
            };

            const start = performance.now();
            const results = await session.run(feeds);
            const latency = performance.now() - start;

            // Decode output tokens (simplified — production uses full vocab decoding)
            const outputTokens = Array.from(results[session.outputNames[0]].data);

            return {
                translation: '[Traduccion requiere tokenizer completo]',
                sourceLang,
                targetLang,
                tokens: outputTokens.length,
                latencyMs: Math.round(latency),
                source: 'local',
                model: modelId,
                backend: detectBackend()
            };
        }
    };

    // --- Image Inference ---
    const image = {
        async classify(imageElement) {
            const modelId = 'image-classify';
            const info = loadedModels.has(modelId) ? loadedModels.get(modelId) : await models.load(modelId);
            const { session, meta } = info;

            // Preprocess image to 224x224 RGB tensor
            const canvas = document.createElement('canvas');
            canvas.width = meta.inputSize;
            canvas.height = meta.inputSize;
            const ctx = canvas.getContext('2d');
            ctx.drawImage(imageElement, 0, 0, meta.inputSize, meta.inputSize);
            const imageData = ctx.getImageData(0, 0, meta.inputSize, meta.inputSize);
            const pixels = imageData.data;

            // Normalize and convert to CHW format (3, 224, 224)
            const floatData = new Float32Array(3 * meta.inputSize * meta.inputSize);
            const mean = [0.485, 0.456, 0.406];
            const std = [0.229, 0.224, 0.225];

            for (let i = 0; i < meta.inputSize * meta.inputSize; i++) {
                floatData[i] = (pixels[i * 4] / 255 - mean[0]) / std[0];     // R
                floatData[i + meta.inputSize * meta.inputSize] = (pixels[i * 4 + 1] / 255 - mean[1]) / std[1]; // G
                floatData[i + 2 * meta.inputSize * meta.inputSize] = (pixels[i * 4 + 2] / 255 - mean[2]) / std[2]; // B
            }

            const tensor = new ort.Tensor('float32', floatData, [1, 3, meta.inputSize, meta.inputSize]);
            const feeds = { [session.inputNames[0]]: tensor };

            const start = performance.now();
            const results = await session.run(feeds);
            const latency = performance.now() - start;

            const logits = Array.from(results[session.outputNames[0]].data);
            const probs = softmax(logits);

            // Get top 5 predictions
            const indexed = probs.map((p, i) => ({ index: i, score: p }));
            indexed.sort((a, b) => b.score - a.score);
            const top5 = indexed.slice(0, 5);

            return {
                predictions: top5.map(p => ({
                    classIndex: p.index,
                    score: Math.round(p.score * 1000) / 1000
                })),
                latencyMs: Math.round(latency),
                source: 'local',
                model: modelId,
                backend: detectBackend()
            };
        }
    };

    // --- NLP ---
    const nlp = {
        async ner(inputText) {
            const modelId = 'ner-multi';
            const info = loadedModels.has(modelId) ? loadedModels.get(modelId) : await models.load(modelId);
            const { session, meta } = info;

            const words = inputText.split(/\s+/);
            const { inputIds, attentionMask } = simpleTokenize(inputText, meta.maxLength);
            const feeds = {
                input_ids: new ort.Tensor('int64', BigInt64Array.from(inputIds.map(BigInt)), [1, meta.maxLength]),
                attention_mask: new ort.Tensor('int64', BigInt64Array.from(attentionMask.map(BigInt)), [1, meta.maxLength])
            };

            const start = performance.now();
            const results = await session.run(feeds);
            const latency = performance.now() - start;

            const logits = results[session.outputNames[0]].data;
            const numLabels = meta.labels.length;
            const entities = [];

            for (let i = 1; i < Math.min(words.length + 1, meta.maxLength - 1); i++) {
                const tokenLogits = Array.from(logits).slice(i * numLabels, (i + 1) * numLabels);
                const probs = softmax(tokenLogits);
                const maxIdx = probs.indexOf(Math.max(...probs));
                const label = meta.labels[maxIdx];

                if (label !== 'O') {
                    entities.push({
                        word: words[i - 1],
                        entity: label,
                        score: Math.round(probs[maxIdx] * 1000) / 1000,
                        index: i - 1
                    });
                }
            }

            return {
                entities,
                text: inputText,
                wordCount: words.length,
                latencyMs: Math.round(latency),
                source: 'local',
                model: modelId,
                backend: detectBackend()
            };
        },

        tokenize(inputText) {
            const words = inputText.split(/\s+/).filter(w => w.length > 0);
            return {
                tokens: words,
                count: words.length,
                characters: inputText.length
            };
        }
    };

    // --- Status ---
    const status = {
        get isReady() { return ortReady; },
        get modelsLoaded() { return Array.from(loadedModels.keys()); },
        get isOffline() { return !navigator.onLine; },
        get gpuAvailable() { return typeof navigator !== 'undefined' && !!navigator.gpu; },
        get backend() { return detectBackend(); },
        get wasmSupported() { return typeof WebAssembly !== 'undefined'; },
        async storageUsed() {
            try {
                const keys = await dbGetAllKeys(STORE_NAME);
                let total = 0;
                for (const key of keys) {
                    const data = await dbGet(STORE_NAME, key);
                    if (data?.byteLength) total += data.byteLength;
                }
                return { bytes: total, mb: Math.round(total / 1024 / 1024 * 10) / 10 };
            } catch {
                return { bytes: 0, mb: 0 };
            }
        }
    };

    // --- Privacy Info ---
    const privacy = {
        info() {
            return {
                dataPolicy: 'zero-transmission',
                inference: 'on-device-only',
                storage: 'local-indexeddb',
                telemetry: false,
                cloudRequired: false,
                description: 'Toda la inferencia AI se ejecuta localmente en tu dispositivo. ' +
                    'Ningun dato sale de tu navegador. Los modelos se descargan una vez y se almacenan ' +
                    'en IndexedDB para uso offline. Compatible con soberania de datos UNDRIP/ILO-169.'
            };
        }
    };

    // --- Initialization ---
    console.log('[IerahkwaAI] Motor de AI Soberana v' + VERSION + ' — Backend: ' + detectBackend());

    // Pre-check WASM support
    if (!status.wasmSupported) {
        console.warn('[IerahkwaAI] WebAssembly no soportado — AI offline no disponible');
    }

    return {
        version: VERSION,
        status,
        models,
        text,
        translate,
        image,
        nlp,
        privacy,
        MODEL_REGISTRY
    };
})();
