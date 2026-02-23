// API Module for Smart POS System

const API = {
  // Base request method
  async request(endpoint, options = {}) {
    const url = `/api${endpoint}`;
    const config = {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers
      },
      ...options
    };

    if (options.body && typeof options.body === 'object') {
      config.body = JSON.stringify(options.body);
    }

    try {
      const response = await fetch(url, config);
      const data = await response.json();

      if (!response.ok) {
        if (response.status === 401) {
          window.location.href = '/login';
          return;
        }
        throw new Error(data.error || 'Request failed');
      }

      return data;
    } catch (error) {
      console.error('API Error:', error);
      throw error;
    }
  },

  // Auth
  auth: {
    check: () => API.request('/auth/check'),
    login: (username, password) => API.request('/auth/login', {
      method: 'POST',
      body: { username, password }
    }),
    logout: () => API.request('/auth/logout', { method: 'POST' }),
    me: () => API.request('/me')
  },

  // Items
  items: {
    getAll: (categoryId) => API.request(`/items${categoryId ? `?category_id=${categoryId}` : ''}`),
    get: (id) => API.request(`/items/${id}`),
    create: (data) => API.request('/items', { method: 'POST', body: data }),
    update: (id, data) => API.request(`/items/${id}`, { method: 'PUT', body: data }),
    delete: (id) => API.request(`/items/${id}`, { method: 'DELETE' }),
    
    // Categories
    getCategories: () => API.request('/items/categories'),
    createCategory: (data) => API.request('/items/categories', { method: 'POST', body: data }),
    updateCategory: (id, data) => API.request(`/items/categories/${id}`, { method: 'PUT', body: data }),
    deleteCategory: (id) => API.request(`/items/categories/${id}`, { method: 'DELETE' })
  },

  // Orders
  orders: {
    getAll: (params = {}) => {
      const query = new URLSearchParams(params).toString();
      return API.request(`/orders${query ? `?${query}` : ''}`);
    },
    get: (id) => API.request(`/orders/${id}`),
    create: (data) => API.request('/orders', { method: 'POST', body: data }),
    update: (id, data) => API.request(`/orders/${id}`, { method: 'PUT', body: data }),
    addItem: (orderId, data) => API.request(`/orders/${orderId}/items`, { method: 'POST', body: data }),
    removeItem: (orderId, itemId) => API.request(`/orders/${orderId}/items/${itemId}`, { method: 'DELETE' }),
    pay: (orderId, data) => API.request(`/orders/${orderId}/pay`, { method: 'POST', body: data })
  },

  // Tables
  tables: {
    getAll: (floor) => API.request(`/tables${floor ? `?floor=${floor}` : ''}`),
    get: (id) => API.request(`/tables/${id}`),
    create: (data) => API.request('/tables', { method: 'POST', body: data }),
    update: (id, data) => API.request(`/tables/${id}`, { method: 'PUT', body: data }),
    updatePosition: (id, pos_x, pos_y) => API.request(`/tables/${id}/position`, {
      method: 'PATCH',
      body: { pos_x, pos_y }
    }),
    updateStatus: (id, status) => API.request(`/tables/${id}/status`, {
      method: 'PATCH',
      body: { status }
    }),
    delete: (id) => API.request(`/tables/${id}`, { method: 'DELETE' }),
    getFloors: () => API.request('/tables/floors/list')
  },

  // Reports
  reports: {
    daily: (date) => API.request(`/reports/daily${date ? `?date=${date}` : ''}`),
    sales: (startDate, endDate) => API.request(`/reports/sales?start_date=${startDate}&end_date=${endDate}`),
    items: (startDate, endDate, categoryId) => {
      let url = `/reports/items?start_date=${startDate}&end_date=${endDate}`;
      if (categoryId) url += `&category_id=${categoryId}`;
      return API.request(url);
    },
    categories: (startDate, endDate) => API.request(`/reports/categories?start_date=${startDate}&end_date=${endDate}`),
    users: (startDate, endDate) => API.request(`/reports/users?start_date=${startDate}&end_date=${endDate}`),
    tables: (startDate, endDate) => API.request(`/reports/tables?start_date=${startDate}&end_date=${endDate}`),
    tax: (startDate, endDate) => API.request(`/reports/tax?start_date=${startDate}&end_date=${endDate}`)
  },

  // Users
  users: {
    getAll: () => API.request('/users'),
    get: (id) => API.request(`/users/${id}`),
    create: (data) => API.request('/users', { method: 'POST', body: data }),
    update: (id, data) => API.request(`/users/${id}`, { method: 'PUT', body: data }),
    delete: (id) => API.request(`/users/${id}`, { method: 'DELETE' }),
    updateProfile: (data) => API.request('/users/profile/me', { method: 'PUT', body: data })
  }
};

// Export for use in other modules
window.API = API;
