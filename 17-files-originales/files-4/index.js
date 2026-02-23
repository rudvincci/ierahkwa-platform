/**
 * @mameynode/sdk v4.2.0
 * Official SDK for Red Soberana Digital de las Américas
 * MameyNode Blockchain · BDET Bank · 42 AI Engines
 */

const { ethers } = require('ethers');
const axios = require('axios');

class MameyNode {
  constructor(config = {}) {
    this.rpcUrl = config.rpcUrl || 'https://chain.soberano.bo';
    this.apiUrl = config.apiUrl || 'https://api.soberano.bo/v1';
    this.provider = new ethers.JsonRpcProvider(this.rpcUrl);
    this.apiKey = config.apiKey || null;
  }

  // Blockchain
  async getBlock(number) { return this.provider.getBlock(number); }
  async getBalance(address) { return this.provider.getBalance(address); }
  async sendTransaction(tx) { return this.provider.sendTransaction(tx); }
  
  // BDET Bank
  async bdetPay(to, amount, platformId) {
    return this._api('POST', '/bdet/payment', { to, amount, platformId });
  }
  async bdetBalance() { return this._api('GET', '/bdet/balance'); }
  async bdetEscrow(seller, amount) {
    return this._api('POST', '/bdet/escrow/create', { seller, amount });
  }
  async bdetForex(from, to, amount) {
    return this._api('POST', '/bdet/forex/quote', { from, to, amount });
  }
  
  // Auth
  async register(data) { return this._api('POST', '/auth/register', data); }
  async login(credentials) { return this._api('POST', '/auth/login', credentials); }
  
  // Atabey Translator
  async translate(text, fromLang, toLang) {
    return this._api('POST', '/atabey/translate', { text, from: fromLang, to: toLang });
  }
  async detectLanguage(text) {
    return this._api('POST', '/atabey/detect', { text });
  }
  
  // MameyAI
  async aiComplete(prompt) { return this._api('POST', '/ai/complete', { prompt }); }
  async aiSummarize(text) { return this._api('POST', '/ai/summarize', { text }); }

  // Internal
  async _api(method, path, data) {
    const headers = { 'Content-Type': 'application/json' };
    if (this.apiKey) headers['Authorization'] = `Bearer ${this.apiKey}`;
    const res = await axios({ method, url: `${this.apiUrl}${path}`, data, headers });
    return res.data;
  }
}

module.exports = { MameyNode };
// Usage: const { MameyNode } = require('@mameynode/sdk');
//        const node = new MameyNode({ apiKey: 'sk-...' });
//        await node.bdetPay('0x...', 100, 7); // Pay artisan on ArtesaníaSoberana
