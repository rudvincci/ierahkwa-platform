/**
 * API Service - Ierahkwa Platform
 */

import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

// Base URL - Configure based on environment
const BASE_URL = __DEV__ 
  ? 'http://localhost:8545' 
  : 'https://api.ierahkwa.gov';

const api = axios.create({
  baseURL: BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor
api.interceptors.request.use(async (config) => {
  const token = await AsyncStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor
api.interceptors.response.use(
  (response) => response.data,
  (error) => {
    console.error('API Error:', error);
    return Promise.reject(error);
  }
);

// ═══════════════════════════════════════════════════════════════════════════════
//                              BLOCKCHAIN
// ═══════════════════════════════════════════════════════════════════════════════

export const getStats = () => api.get('/api/v1/stats');

export const getBlockNumber = () => api.post('/rpc', {
  jsonrpc: '2.0',
  method: 'eth_blockNumber',
  params: [],
  id: 1,
});

export const getBalance = (address) => api.post('/rpc', {
  jsonrpc: '2.0',
  method: 'eth_getBalance',
  params: [address, 'latest'],
  id: 1,
});

// ═══════════════════════════════════════════════════════════════════════════════
//                              TOKENS
// ═══════════════════════════════════════════════════════════════════════════════

export const getTokens = () => api.get('/api/v1/tokens').then(r => r.tokens);

export const getToken = (symbol) => api.get(`/api/v1/tokens/${symbol}`);

export const createToken = (data) => api.post('/api/v1/tokens/create', data);

export const getCustomTokens = () => api.get('/api/v1/tokens/custom');

// ═══════════════════════════════════════════════════════════════════════════════
//                              BRIDGE
// ═══════════════════════════════════════════════════════════════════════════════

export const getBridgeChains = () => api.get('/api/v1/bridge/chains');

export const getBridgeTokens = () => api.get('/api/v1/bridge/tokens');

export const initiateBridgeDeposit = (data) => api.post('/api/v1/bridge/deposit', data);

export const initiateBridgeWithdraw = (data) => api.post('/api/v1/bridge/withdraw', data);

export const getBridgeStatus = (id) => api.get(`/api/v1/bridge/status/${id}`);

export const getBridgeHistory = () => api.get('/api/v1/bridge/history');

// ═══════════════════════════════════════════════════════════════════════════════
//                              VOTING
// ═══════════════════════════════════════════════════════════════════════════════

export const getProposals = () => api.get('/api/v1/voting/proposals');

export const getProposal = (id) => api.get(`/api/v1/voting/proposals/${id}`);

export const createProposal = (data) => api.post('/api/v1/voting/proposals', data);

export const castVote = (data) => api.post('/api/v1/voting/vote', data);

// ═══════════════════════════════════════════════════════════════════════════════
//                              GAMIFICATION
// ═══════════════════════════════════════════════════════════════════════════════

export const getProfile = (address) => api.get(`/api/v1/gamification/profile/${address}`);

export const claimDailyReward = (address) => api.post('/api/v1/gamification/daily', { address });

export const getAchievements = () => api.get('/api/v1/gamification/achievements');

export const getLeaderboard = () => api.get('/api/v1/gamification/leaderboard');

// ═══════════════════════════════════════════════════════════════════════════════
//                              ANALYTICS
// ═══════════════════════════════════════════════════════════════════════════════

export const trackPageView = (page) => api.post('/api/v1/analytics/pageview', { page });

export const trackEvent = (category, action, label) => 
  api.post('/api/v1/analytics/event', { category, action, label });

export const getRealtime = () => api.get('/api/v1/analytics/realtime');

// ═══════════════════════════════════════════════════════════════════════════════
//                              i18n
// ═══════════════════════════════════════════════════════════════════════════════

export const getTranslations = (lang) => api.get(`/api/v1/i18n/${lang}`);

export const getLanguages = () => api.get('/api/v1/i18n/languages');

// ═══════════════════════════════════════════════════════════════════════════════
//                              NOTIFICATIONS
// ═══════════════════════════════════════════════════════════════════════════════

export const subscribe = (email, address) => 
  api.post('/api/v1/notifications/subscribe', { email, address });

export const unsubscribe = (email) => 
  api.post('/api/v1/notifications/unsubscribe', { email });

// Default export
export default {
  getStats,
  getTokens,
  getToken,
  createToken,
  getBridgeChains,
  getBridgeTokens,
  initiateBridgeDeposit,
  getProposals,
  castVote,
  getProfile,
  claimDailyReward,
  getLeaderboard,
  getRealtime,
  getTranslations,
};
