/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  API SERVICE - Axios Configuration
 * ═══════════════════════════════════════════════════════════════════════════════
 */

import axios from 'axios';
import * as SecureStore from 'expo-secure-store';

const API_URL = __DEV__ 
  ? 'http://localhost:8545/api/v1'
  : 'https://api.ierahkwa.io/api/v1';

const api = axios.create({
  baseURL: API_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor
api.interceptors.request.use(
  async (config) => {
    const token = await SecureStore.getItemAsync('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      try {
        const refreshToken = await SecureStore.getItemAsync('refreshToken');
        
        if (refreshToken) {
          const response = await axios.post(`${API_URL}/auth/refresh`, {
            refreshToken
          });
          
          const { accessToken } = response.data.data;
          await SecureStore.setItemAsync('accessToken', accessToken);
          
          api.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
          
          return api(originalRequest);
        }
      } catch (refreshError) {
        // Logout user
        await SecureStore.deleteItemAsync('accessToken');
        await SecureStore.deleteItemAsync('refreshToken');
      }
    }
    
    return Promise.reject(error);
  }
);

export default api;

// API Endpoints
export const authAPI = {
  login: (data: { email: string; password: string }) => api.post('/auth/login', data),
  register: (data: any) => api.post('/auth/register', data),
  verify2FA: (data: { tempToken: string; code: string }) => api.post('/auth/verify-2fa', data),
  logout: () => api.post('/auth/logout'),
  me: () => api.get('/auth/me'),
};

export const walletAPI = {
  getBalance: () => api.get('/wallet/balance'),
  getTransactions: (params?: any) => api.get('/wallet/transactions', { params }),
  send: (data: { to: string; amount: string; currency?: string }) => api.post('/wallet/send', data),
  getAddress: () => api.get('/wallet/address'),
};

export const tokensAPI = {
  getAll: () => api.get('/tokens'),
  getBalance: (symbol: string) => api.get(`/tokens/${symbol}/balance`),
  transfer: (data: { symbol: string; to: string; amount: string }) => api.post('/tokens/transfer', data),
};

export const governmentAPI = {
  getServices: () => api.get('/government/services'),
  getDocuments: () => api.get('/government/documents'),
  submitApplication: (data: any) => api.post('/government/applications', data),
  getApplicationStatus: (id: string) => api.get(`/government/applications/${id}`),
};

export const aiAPI = {
  chat: (data: { sessionId?: string; message: string; provider?: string }) => api.post('/ai/chat', data),
  government: (data: { query: string; context?: any }) => api.post('/ai/government', data),
  banking: (data: { query: string }) => api.post('/ai/banking', data),
};
