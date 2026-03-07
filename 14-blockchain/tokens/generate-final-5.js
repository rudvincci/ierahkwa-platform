#!/usr/bin/env node
const fs = require('fs');
const path = require('path');

const FINAL = [
  { name: "Tucano de Colombia", symbol: "TUCANOCO", region: "Amazonia", country: "CO", lang: "Tucano" },
  { name: "Embera Katío", symbol: "KATIO", region: "Amazonia", country: "CO", lang: "Embera" },
  { name: "Zenú de Córdoba", symbol: "ZENUCOR", region: "Caribe", country: "CO", lang: "Zenú" },
  { name: "Pasto (Quillacinga)", symbol: "PASTO", region: "Andes", country: "CO/EC", lang: "Pasto" },
  { name: "Coconuco", symbol: "COCONUCO", region: "Andes", country: "CO", lang: "Coconuco" },
];

const baseDir = path.join(__dirname, 'nations');
const manifestPath = path.join(baseDir, 'MANIFEST.json');
const existing = JSON.parse(fs.readFileSync(manifestPath, 'utf8'));
let startId = existing.tokens.length;

FINAL.forEach((nation) => {
  startId++;
  const id = String(startId).padStart(4, '0');
  const symbol = `SNT-${nation.symbol}`;
  const contractAddr = `0x574${id}${'0'.repeat(36)}`;
  const token = {
    id, name: nation.name, symbol, standard: "SNT (Sovereign Nation Token)",
    description: `Token soberano de la nacion ${nation.name}.`,
    region: nation.region, country: nation.country, language: nation.lang,
    blockchain: { network: "MameyNode", chainId: 574, contract: contractAddr, standard: "SNT-574", centralBank: "BDET Bank", nativeCurrency: "WAMPUM (WPM)" },
    decimals: 18, totalSupply: "1000000000", status: "pre-minted",
    activationCondition: "Sovereign recognition signature by tribal council"
  };
  fs.writeFileSync(path.join(baseDir, `${id}-${symbol}.json`), JSON.stringify(token, null, 2));
  existing.tokens.push({ id, symbol, name: nation.name, region: nation.region, country: nation.country, language: nation.lang, contract: contractAddr, status: "pre-minted" });
});

existing.totalNations = existing.tokens.length;
existing.generatedAt = new Date().toISOString();
fs.writeFileSync(manifestPath, JSON.stringify(existing, null, 2));
console.log(`Total final: ${existing.totalNations} SNT tokens (target: 574)`);
