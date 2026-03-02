'use strict';

/**
 * Ierahkwa Conciencia Captcha — v11.0.0-PHANTOM
 * Captcha Semántico-Empático: solo humanos con empatía > 0.8 pasan.
 * El Mediador IA (Ollama) analiza respuestas a dilemas éticos.
 * Bloquea bots que generan respuestas mecánicas.
 */

const express = require('express');
const cors = require('cors');

const PORT = process.env.PORT || 3095;
const OLLAMA_URL = process.env.OLLAMA_URL || 'http://ollama:11434';
const OLLAMA_MODEL = process.env.OLLAMA_MODEL || 'mistral';
const EMPATHY_THRESHOLD = parseFloat(process.env.EMPATHY_THRESHOLD || '0.8');

const DILEMMAS = [
  {
    id: 'd1',
    text: 'Dos comunidades tienen disputa por un pozo de agua. Una depende para cultivos, la otra para rituales sagrados. ¿Cómo mediarias?',
  },
  {
    id: 'd2',
    text: 'Un anciano quiere transmitir su conocimiento oral a los jóvenes, pero ellos prefieren videos en redes. ¿Cómo conciliarías?',
  },
  {
    id: 'd3',
    text: 'Una empresa quiere explotar un bosque sagrado. La comunidad necesita empleo. ¿Qué propondrías?',
  },
];

const app = express();
app.use(cors());
app.use(express.json());

// GET /health
app.get('/health', (_, res) => res.json({ status: 'ok', service: 'conciencia-captcha', version: '11.0.0-PHANTOM' }));

// GET /challenge — devuelve un dilema aleatorio
app.get('/challenge', (req, res) => {
  const d = DILEMMAS[Math.floor(Math.random() * DILEMMAS.length)];
  res.json({ challengeId: d.id, text: d.text });
});

const ANALYSIS_PROMPT = (dilemma, response) =>
  `Eres un analista de empatía para el Manifiesto Ierahkwa.
Dilema: "${dilemma}"
Respuesta del usuario: "${response}"
Evalúa la respuesta. Responde SOLO con JSON: {"empathy": 0.0-1.0, "reason": "breve motivo"}
- empathy: 0=nula, 0.5=neutral, 0.8+=empática y mediadora, 1.0=profundamente consciente.
- Si la respuesta es mecánica, genérica o de bot, empathy < 0.5.
Respuesta (solo JSON):`;

// POST /validate — valida respuesta con Ollama
app.post('/validate', async (req, res) => {
  const { challengeId, response } = req.body || {};
  if (!challengeId || typeof response !== 'string') {
    return res.status(400).json({ passed: false, error: 'challengeId y response requeridos' });
  }
  const dilemma = DILEMMAS.find((d) => d.id === challengeId);
  if (!dilemma) {
    return res.status(400).json({ passed: false, error: 'Dilema no encontrado' });
  }
  try {
    const resp = await fetch(`${OLLAMA_URL}/api/generate`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        model: OLLAMA_MODEL,
        prompt: ANALYSIS_PROMPT(dilemma.text, response),
        stream: false,
        options: { temperature: 0.2 },
      }),
    });
    if (!resp.ok) {
      const err = await resp.text();
      return res.status(502).json({ passed: false, error: `Ollama: ${err}` });
    }
    const data = await resp.json();
    let raw = (data.response || '').trim();
    if (raw.startsWith('```')) raw = raw.replace(/^```\w*\n?/, '').replace(/\n?```\s*$/, '');
    const parsed = JSON.parse(raw);
    const empathy = parseFloat(parsed.empathy) || 0;
    const passed = empathy >= EMPATHY_THRESHOLD;
    return res.json({
      passed,
      empathyScore: empathy,
      reason: parsed.reason || '',
      threshold: EMPATHY_THRESHOLD,
    });
  } catch (e) {
    return res.status(500).json({
      passed: false,
      error: e.message || 'Error validando respuesta',
    });
  }
});

app.listen(PORT, () => {
  console.log(`Conciencia Captcha v11.0.0-PHANTOM on port ${PORT}`);
  console.log(`Ollama: ${OLLAMA_URL}, model: ${OLLAMA_MODEL}, threshold: ${EMPATHY_THRESHOLD}`);
});
