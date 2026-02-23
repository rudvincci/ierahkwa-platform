/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  AUTH STORE - Zustand State Management
 * ═══════════════════════════════════════════════════════════════════════════════
 */

import { create } from 'zustand';
import * as SecureStore from 'expo-secure-store';
import api from '../services/api';

interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  citizenId: string;
  walletAddress: string;
  role: string;
}

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  
  login: (email: string, password: string) => Promise<boolean>;
  register: (data: any) => Promise<boolean>;
  logout: () => Promise<void>;
  checkAuth: () => Promise<void>;
  verify2FA: (tempToken: string, code: string) => Promise<boolean>;
  clearError: () => void;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  user: null,
  token: null,
  isAuthenticated: false,
  isLoading: false,
  error: null,

  login: async (email: string, password: string) => {
    set({ isLoading: true, error: null });
    try {
      const response = await api.post('/auth/login', { email, password });
      
      if (response.data.requiresTwoFactor) {
        set({ isLoading: false });
        return false; // Needs 2FA
      }

      const { accessToken, refreshToken, user } = response.data.data;
      
      await SecureStore.setItemAsync('accessToken', accessToken);
      await SecureStore.setItemAsync('refreshToken', refreshToken);
      
      api.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
      
      set({
        user,
        token: accessToken,
        isAuthenticated: true,
        isLoading: false
      });
      
      return true;
    } catch (error: any) {
      set({
        error: error.response?.data?.error || 'Login failed',
        isLoading: false
      });
      return false;
    }
  },

  register: async (data: any) => {
    set({ isLoading: true, error: null });
    try {
      const response = await api.post('/auth/register', data);
      set({ isLoading: false });
      return true;
    } catch (error: any) {
      set({
        error: error.response?.data?.error || 'Registration failed',
        isLoading: false
      });
      return false;
    }
  },

  verify2FA: async (tempToken: string, code: string) => {
    set({ isLoading: true, error: null });
    try {
      const response = await api.post('/auth/verify-2fa', { tempToken, code });
      const { accessToken, refreshToken, user } = response.data.data;
      
      await SecureStore.setItemAsync('accessToken', accessToken);
      await SecureStore.setItemAsync('refreshToken', refreshToken);
      
      api.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
      
      set({
        user,
        token: accessToken,
        isAuthenticated: true,
        isLoading: false
      });
      
      return true;
    } catch (error: any) {
      set({
        error: error.response?.data?.error || '2FA verification failed',
        isLoading: false
      });
      return false;
    }
  },

  logout: async () => {
    try {
      await api.post('/auth/logout');
    } catch (e) {}
    
    await SecureStore.deleteItemAsync('accessToken');
    await SecureStore.deleteItemAsync('refreshToken');
    delete api.defaults.headers.common['Authorization'];
    
    set({
      user: null,
      token: null,
      isAuthenticated: false
    });
  },

  checkAuth: async () => {
    try {
      const token = await SecureStore.getItemAsync('accessToken');
      
      if (!token) {
        set({ isAuthenticated: false });
        return;
      }

      api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      
      const response = await api.get('/auth/me');
      
      set({
        user: response.data.data,
        token,
        isAuthenticated: true
      });
    } catch (error) {
      await SecureStore.deleteItemAsync('accessToken');
      await SecureStore.deleteItemAsync('refreshToken');
      set({ isAuthenticated: false });
    }
  },

  clearError: () => set({ error: null })
}));
