// Smart POS System - Main Application

class SmartPOS {
  constructor() {
    this.user = null;
    this.cart = [];
    this.selectedTable = null;
    this.items = [];
    this.categories = [];
    this.tables = [];
    this.currentView = 'sale';
    this.currencySymbol = '$';
    this.discount = { amount: 0, type: 'fixed' };
    
    this.init();
  }

  async init() {
    // Check authentication
    try {
      const authData = await API.auth.check();
      if (!authData.authenticated) {
        window.location.href = '/login';
        return;
      }
      this.user = authData.user;
    } catch (error) {
      window.location.href = '/login';
      return;
    }

    // Initialize i18n
    i18n.init();
    if (this.user.language) {
      i18n.setLanguage(this.user.language);
    }

    // Display user info
    document.getElementById('current-user').textContent = this.user.full_name || this.user.username;

    // Load initial data
    await this.loadCategories();
    await this.loadItems();
    
    // Setup event listeners
    this.setupEventListeners();
    
    // Show sale view by default
    this.showView('sale');
  }

  setupEventListeners() {
    // Navigation
    document.querySelectorAll('.nav-item').forEach(item => {
      item.addEventListener('click', (e) => {
        e.preventDefault();
        const view = item.dataset.view;
        const permission = item.dataset.permission;
        
        // Check permission
        if (permission && this.user.role !== 'admin' && !this.user.permissions[permission]) {
          this.showToast('Access denied', 'error');
          return;
        }
        
        this.showView(view);
      });
    });

    // Logout
    document.getElementById('btn-logout').addEventListener('click', async () => {
      await API.auth.logout();
      window.location.href = '/login';
    });

    // Language
    document.getElementById('btn-language').addEventListener('click', () => {
      this.showLanguageModal();
    });

    // Category buttons
    document.getElementById('categories-bar').addEventListener('click', (e) => {
      const btn = e.target.closest('.category-btn');
      if (btn) {
        document.querySelectorAll('.category-btn').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        this.filterItems(btn.dataset.category);
      }
    });

    // Item search
    document.getElementById('item-search').addEventListener('input', (e) => {
      this.searchItems(e.target.value);
    });

    // Items grid click
    document.getElementById('items-grid').addEventListener('click', (e) => {
      const card = e.target.closest('.item-card');
      if (card) {
        const itemId = parseInt(card.dataset.id);
        this.addToCart(itemId);
      }
    });

    // Cart actions
    document.getElementById('btn-clear-cart').addEventListener('click', () => {
      this.clearCart();
    });

    document.getElementById('btn-discount').addEventListener('click', () => {
      this.showDiscountModal();
    });

    document.getElementById('btn-checkout').addEventListener('click', () => {
      this.showPaymentModal();
    });

    document.getElementById('btn-select-table').addEventListener('click', () => {
      this.showTableSelectModal();
    });

    document.getElementById('btn-clear-table').addEventListener('click', () => {
      this.selectedTable = null;
      document.getElementById('cart-table-info').style.display = 'none';
    });

    // Modal close
    document.getElementById('modal-close').addEventListener('click', () => {
      this.closeModal();
    });

    document.getElementById('modal-overlay').addEventListener('click', (e) => {
      if (e.target.id === 'modal-overlay') {
        this.closeModal();
      }
    });

    // Cart items (event delegation)
    document.getElementById('cart-items').addEventListener('click', (e) => {
      const target = e.target.closest('button');
      if (!target) return;

      const itemEl = target.closest('.cart-item');
      if (!itemEl) return;

      const index = parseInt(itemEl.dataset.index);

      if (target.classList.contains('qty-minus')) {
        this.updateCartQuantity(index, -1);
      } else if (target.classList.contains('qty-plus')) {
        this.updateCartQuantity(index, 1);
      } else if (target.classList.contains('cart-item-remove')) {
        this.removeFromCart(index);
      }
    });

    // Reports
    document.getElementById('btn-generate-report')?.addEventListener('click', () => {
      this.generateReport();
    });

    document.querySelectorAll('.tab-btn').forEach(btn => {
      btn.addEventListener('click', () => {
        document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        this.generateReport(btn.dataset.report);
      });
    });

    // Tables view
    document.getElementById('btn-add-table')?.addEventListener('click', () => {
      this.showTableModal();
    });

    // Items management
    document.getElementById('btn-add-item')?.addEventListener('click', () => {
      this.showItemModal();
    });

    document.getElementById('btn-add-category')?.addEventListener('click', () => {
      this.showCategoryModal();
    });

    // Users management
    document.getElementById('btn-add-user')?.addEventListener('click', () => {
      this.showUserModal();
    });

    // Orders date filter
    document.getElementById('orders-date')?.addEventListener('change', () => {
      this.loadOrders();
    });

    document.getElementById('orders-status')?.addEventListener('change', () => {
      this.loadOrders();
    });

    // Set default date for reports
    const today = new Date().toISOString().slice(0, 10);
    const startDateEl = document.getElementById('report-start-date');
    const endDateEl = document.getElementById('report-end-date');
    const ordersDateEl = document.getElementById('orders-date');
    
    if (startDateEl) startDateEl.value = today;
    if (endDateEl) endDateEl.value = today;
    if (ordersDateEl) ordersDateEl.value = today;
  }

  // View Management
  showView(viewName) {
    document.querySelectorAll('.view').forEach(v => v.classList.remove('active'));
    document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));

    const view = document.getElementById(`view-${viewName}`);
    const navItem = document.querySelector(`.nav-item[data-view="${viewName}"]`);

    if (view) view.classList.add('active');
    if (navItem) navItem.classList.add('active');

    this.currentView = viewName;

    // Load view-specific data
    switch (viewName) {
      case 'tables':
        this.loadTables();
        break;
      case 'orders':
        this.loadOrders();
        break;
      case 'reports':
        this.generateReport('daily');
        break;
      case 'items':
        this.loadItemsManagement();
        break;
      case 'users':
        this.loadUsers();
        break;
    }
  }

  // Categories & Items
  async loadCategories() {
    try {
      this.categories = await API.items.getCategories();
      this.renderCategories();
    } catch (error) {
      this.showToast('Failed to load categories', 'error');
    }
  }

  renderCategories() {
    const bar = document.getElementById('categories-bar');
    bar.innerHTML = `
      <button class="category-btn active" data-category="all">
        <i class="fas fa-th"></i> ${i18n.t('all')}
      </button>
    `;

    this.categories.forEach(cat => {
      bar.innerHTML += `
        <button class="category-btn" data-category="${cat.id}" style="--cat-color: ${cat.color}">
          <i class="fas fa-${cat.icon || 'folder'}"></i> ${cat.name}
        </button>
      `;
    });
  }

  async loadItems() {
    try {
      this.items = await API.items.getAll();
      this.renderItems();
    } catch (error) {
      this.showToast('Failed to load items', 'error');
    }
  }

  renderItems(filteredItems = null) {
    const grid = document.getElementById('items-grid');
    const itemsToRender = filteredItems || this.items;

    if (itemsToRender.length === 0) {
      grid.innerHTML = '<p style="text-align: center; color: var(--gray-500); grid-column: 1/-1;">No items found</p>';
      return;
    }

    grid.innerHTML = itemsToRender.map(item => `
      <div class="item-card" data-id="${item.id}">
        <div class="item-icon" style="color: ${item.category_color || 'var(--primary)'}">
          <i class="fas fa-utensils"></i>
        </div>
        <div class="item-name">${item.name}</div>
        <div class="item-price">${this.currencySymbol}${item.price.toFixed(2)}</div>
      </div>
    `).join('');
  }

  filterItems(categoryId) {
    if (categoryId === 'all') {
      this.renderItems();
    } else {
      const filtered = this.items.filter(i => i.category_id === parseInt(categoryId));
      this.renderItems(filtered);
    }
  }

  searchItems(query) {
    if (!query.trim()) {
      this.renderItems();
      return;
    }

    const filtered = this.items.filter(item => 
      item.name.toLowerCase().includes(query.toLowerCase()) ||
      (item.name_ar && item.name_ar.includes(query))
    );
    this.renderItems(filtered);
  }

  // Cart Management
  addToCart(itemId) {
    const item = this.items.find(i => i.id === itemId);
    if (!item) return;

    const existingIndex = this.cart.findIndex(i => i.id === itemId);
    
    if (existingIndex >= 0) {
      this.cart[existingIndex].quantity++;
    } else {
      this.cart.push({
        id: item.id,
        name: item.name,
        price: item.price,
        tax_rate: item.tax_rate,
        quantity: 1
      });
    }

    this.renderCart();
    this.showToast(`Added ${item.name}`, 'success');
  }

  updateCartQuantity(index, delta) {
    if (this.cart[index]) {
      this.cart[index].quantity += delta;
      if (this.cart[index].quantity <= 0) {
        this.cart.splice(index, 1);
      }
      this.renderCart();
    }
  }

  removeFromCart(index) {
    this.cart.splice(index, 1);
    this.renderCart();
  }

  clearCart() {
    this.cart = [];
    this.discount = { amount: 0, type: 'fixed' };
    this.renderCart();
  }

  renderCart() {
    const container = document.getElementById('cart-items');
    const checkoutBtn = document.getElementById('btn-checkout');

    if (this.cart.length === 0) {
      container.innerHTML = `
        <div class="cart-empty">
          <i class="fas fa-shopping-basket"></i>
          <p>${i18n.t('cart_empty')}</p>
        </div>
      `;
      checkoutBtn.disabled = true;
      this.updateCartTotals();
      return;
    }

    container.innerHTML = this.cart.map((item, index) => `
      <div class="cart-item" data-index="${index}">
        <div class="cart-item-info">
          <div class="cart-item-name">${item.name}</div>
          <div class="cart-item-price">${this.currencySymbol}${item.price.toFixed(2)}</div>
        </div>
        <div class="cart-item-qty">
          <button class="qty-minus">-</button>
          <span>${item.quantity}</span>
          <button class="qty-plus">+</button>
        </div>
        <div class="cart-item-total">${this.currencySymbol}${(item.price * item.quantity).toFixed(2)}</div>
        <button class="cart-item-remove btn-icon"><i class="fas fa-trash"></i></button>
      </div>
    `).join('');

    checkoutBtn.disabled = false;
    this.updateCartTotals();
  }

  updateCartTotals() {
    const subtotal = this.cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const tax = this.cart.reduce((sum, item) => sum + (item.price * item.quantity * (item.tax_rate || 0)), 0);
    
    let discountAmount = 0;
    if (this.discount.type === 'percentage') {
      discountAmount = subtotal * (this.discount.amount / 100);
    } else {
      discountAmount = this.discount.amount;
    }

    const total = subtotal + tax - discountAmount;

    document.getElementById('cart-subtotal').textContent = `${this.currencySymbol}${subtotal.toFixed(2)}`;
    document.getElementById('cart-tax').textContent = `${this.currencySymbol}${tax.toFixed(2)}`;
    document.getElementById('cart-total').textContent = `${this.currencySymbol}${total.toFixed(2)}`;

    const discountRow = document.querySelector('.discount-row');
    if (discountAmount > 0) {
      discountRow.style.display = 'flex';
      document.getElementById('cart-discount').textContent = `-${this.currencySymbol}${discountAmount.toFixed(2)}`;
    } else {
      discountRow.style.display = 'none';
    }
  }

  getCartTotal() {
    const subtotal = this.cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const tax = this.cart.reduce((sum, item) => sum + (item.price * item.quantity * (item.tax_rate || 0)), 0);
    
    let discountAmount = 0;
    if (this.discount.type === 'percentage') {
      discountAmount = subtotal * (this.discount.amount / 100);
    } else {
      discountAmount = this.discount.amount;
    }

    return subtotal + tax - discountAmount;
  }

  // Tables
  async loadTables() {
    try {
      this.tables = await API.tables.getAll();
      this.renderTables();
    } catch (error) {
      this.showToast('Failed to load tables', 'error');
    }
  }

  renderTables() {
    const canvas = document.getElementById('tables-canvas');
    canvas.innerHTML = '';

    this.tables.forEach(table => {
      const el = document.createElement('div');
      el.className = `table-item ${table.shape} ${table.status}`;
      el.dataset.id = table.id;
      el.style.left = `${table.pos_x}px`;
      el.style.top = `${table.pos_y}px`;
      el.style.width = `${table.width}px`;
      el.style.height = `${table.height}px`;

      el.innerHTML = `
        <div class="table-name">${table.name}</div>
        <div class="table-capacity"><i class="fas fa-user"></i> ${table.capacity}</div>
        ${table.order_total ? `<div class="table-amount">${this.currencySymbol}${table.order_total.toFixed(2)}</div>` : ''}
      `;

      // Make draggable
      this.makeTableDraggable(el);

      // Click to view/edit
      el.addEventListener('dblclick', () => {
        this.showTableModal(table);
      });

      canvas.appendChild(el);
    });
  }

  makeTableDraggable(el) {
    let isDragging = false;
    let startX, startY, origX, origY;

    el.addEventListener('mousedown', (e) => {
      if (e.button !== 0) return;
      isDragging = true;
      el.classList.add('dragging');
      startX = e.clientX;
      startY = e.clientY;
      origX = parseInt(el.style.left);
      origY = parseInt(el.style.top);
    });

    document.addEventListener('mousemove', (e) => {
      if (!isDragging) return;
      const dx = e.clientX - startX;
      const dy = e.clientY - startY;
      el.style.left = `${origX + dx}px`;
      el.style.top = `${origY + dy}px`;
    });

    document.addEventListener('mouseup', async () => {
      if (!isDragging) return;
      isDragging = false;
      el.classList.remove('dragging');
      
      // Save position
      const id = parseInt(el.dataset.id);
      const pos_x = parseInt(el.style.left);
      const pos_y = parseInt(el.style.top);
      
      try {
        await API.tables.updatePosition(id, pos_x, pos_y);
      } catch (error) {
        console.error('Failed to save table position');
      }
    });
  }

  // Orders
  async loadOrders() {
    const date = document.getElementById('orders-date').value;
    const status = document.getElementById('orders-status').value;

    try {
      const orders = await API.orders.getAll({ date, status });
      this.renderOrders(orders);
    } catch (error) {
      this.showToast('Failed to load orders', 'error');
    }
  }

  renderOrders(orders) {
    const container = document.getElementById('orders-list');

    if (orders.length === 0) {
      container.innerHTML = '<p style="text-align: center; color: var(--gray-500);">No orders found</p>';
      return;
    }

    container.innerHTML = orders.map(order => `
      <div class="order-card" data-id="${order.id}">
        <div class="order-info">
          <h3>${order.order_number}</h3>
          <p>${order.table_name || 'No table'} • ${new Date(order.created_at).toLocaleTimeString()}</p>
        </div>
        <span class="order-status ${order.status}">${order.status}</span>
        <div class="order-total">${this.currencySymbol}${order.total.toFixed(2)}</div>
        <div class="order-actions">
          <button class="btn-icon" onclick="app.viewOrder(${order.id})"><i class="fas fa-eye"></i></button>
        </div>
      </div>
    `).join('');
  }

  async viewOrder(orderId) {
    try {
      const order = await API.orders.get(orderId);
      this.showOrderModal(order);
    } catch (error) {
      this.showToast('Failed to load order', 'error');
    }
  }

  // Reports
  async generateReport(type = 'daily') {
    const startDate = document.getElementById('report-start-date').value;
    const endDate = document.getElementById('report-end-date').value;
    const container = document.getElementById('reports-content');

    try {
      let data;
      switch (type) {
        case 'daily':
          data = await API.reports.daily(startDate);
          this.renderDailyReport(data, container);
          break;
        case 'sales':
          data = await API.reports.sales(startDate, endDate);
          this.renderSalesReport(data, container);
          break;
        case 'items':
          data = await API.reports.items(startDate, endDate);
          this.renderItemsReport(data, container);
          break;
        case 'categories':
          data = await API.reports.categories(startDate, endDate);
          this.renderCategoriesReport(data, container);
          break;
      }
    } catch (error) {
      container.innerHTML = '<p style="color: var(--danger);">Failed to generate report</p>';
    }
  }

  renderDailyReport(data, container) {
    const { summary, top_items, hourly_breakdown } = data;
    
    container.innerHTML = `
      <div class="report-summary">
        <div class="summary-card">
          <div class="value">${summary.total_orders}</div>
          <div class="label">${i18n.t('total_orders')}</div>
        </div>
        <div class="summary-card">
          <div class="value">${this.currencySymbol}${summary.revenue.toFixed(2)}</div>
          <div class="label">${i18n.t('total_revenue')}</div>
        </div>
        <div class="summary-card">
          <div class="value">${this.currencySymbol}${(summary.revenue / (summary.total_orders || 1)).toFixed(2)}</div>
          <div class="label">${i18n.t('avg_order')}</div>
        </div>
        <div class="summary-card">
          <div class="value">${this.currencySymbol}${summary.tax_total.toFixed(2)}</div>
          <div class="label">${i18n.t('tax')}</div>
        </div>
      </div>
      
      <h3>${i18n.t('top_items')}</h3>
      <table class="data-table" style="margin-top: 10px;">
        <thead>
          <tr>
            <th>${i18n.t('name')}</th>
            <th>${i18n.t('quantity')}</th>
            <th>Revenue</th>
          </tr>
        </thead>
        <tbody>
          ${top_items.map(item => `
            <tr>
              <td>${item.item_name}</td>
              <td>${item.quantity}</td>
              <td>${this.currencySymbol}${item.revenue.toFixed(2)}</td>
            </tr>
          `).join('')}
        </tbody>
      </table>
      
      <h3 style="margin-top: 20px;">${i18n.t('sales_by_hour')}</h3>
      <div class="report-chart">
        ${hourly_breakdown.map(h => {
          const maxRevenue = Math.max(...hourly_breakdown.map(x => x.revenue)) || 1;
          const height = (h.revenue / maxRevenue * 250) + 10;
          return `
            <div class="chart-bar" style="height: ${height}px;">
              <span class="bar-value">${this.currencySymbol}${h.revenue.toFixed(0)}</span>
              <span class="bar-label">${h.hour}:00</span>
            </div>
          `;
        }).join('')}
      </div>
    `;
  }

  renderSalesReport(data, container) {
    const { daily_sales, totals } = data;
    
    container.innerHTML = `
      <div class="report-summary">
        <div class="summary-card">
          <div class="value">${totals.total_orders}</div>
          <div class="label">${i18n.t('total_orders')}</div>
        </div>
        <div class="summary-card">
          <div class="value">${this.currencySymbol}${totals.revenue.toFixed(2)}</div>
          <div class="label">${i18n.t('total_revenue')}</div>
        </div>
      </div>
      
      <table class="data-table" style="margin-top: 20px;">
        <thead>
          <tr>
            <th>${i18n.t('date')}</th>
            <th>${i18n.t('orders')}</th>
            <th>Revenue</th>
            <th>${i18n.t('tax')}</th>
          </tr>
        </thead>
        <tbody>
          ${daily_sales.map(day => `
            <tr>
              <td>${day.date}</td>
              <td>${day.orders}</td>
              <td>${this.currencySymbol}${day.revenue.toFixed(2)}</td>
              <td>${this.currencySymbol}${day.tax.toFixed(2)}</td>
            </tr>
          `).join('')}
        </tbody>
      </table>
    `;
  }

  renderItemsReport(data, container) {
    container.innerHTML = `
      <table class="data-table">
        <thead>
          <tr>
            <th>${i18n.t('name')}</th>
            <th>${i18n.t('category')}</th>
            <th>Qty Sold</th>
            <th>Revenue</th>
          </tr>
        </thead>
        <tbody>
          ${data.items.map(item => `
            <tr>
              <td>${item.name}</td>
              <td>${item.category || '-'}</td>
              <td>${item.quantity_sold}</td>
              <td>${this.currencySymbol}${item.revenue.toFixed(2)}</td>
            </tr>
          `).join('')}
        </tbody>
      </table>
    `;
  }

  renderCategoriesReport(data, container) {
    container.innerHTML = `
      <table class="data-table">
        <thead>
          <tr>
            <th>${i18n.t('category')}</th>
            <th>${i18n.t('orders')}</th>
            <th>Items Sold</th>
            <th>Revenue</th>
          </tr>
        </thead>
        <tbody>
          ${data.categories.map(cat => `
            <tr>
              <td><span style="color: ${cat.color}"><i class="fas fa-circle"></i></span> ${cat.name}</td>
              <td>${cat.orders}</td>
              <td>${cat.items_sold}</td>
              <td>${this.currencySymbol}${cat.revenue.toFixed(2)}</td>
            </tr>
          `).join('')}
        </tbody>
      </table>
    `;
  }

  // Items Management
  async loadItemsManagement() {
    await this.loadCategories();
    await this.loadItems();
    
    // Render categories list
    const catList = document.getElementById('manage-categories');
    catList.innerHTML = `
      <h3>Categories</h3>
      ${this.categories.map(cat => `
        <div class="category-item" data-id="${cat.id}">
          <span class="category-color" style="background: ${cat.color}"></span>
          ${cat.name}
        </div>
      `).join('')}
    `;

    // Render items table
    const tbody = document.getElementById('items-table-body');
    tbody.innerHTML = this.items.map(item => `
      <tr>
        <td>${item.name}</td>
        <td>${item.category_name || '-'}</td>
        <td>${this.currencySymbol}${item.price.toFixed(2)}</td>
        <td>${(item.tax_rate * 100).toFixed(0)}%</td>
        <td class="actions">
          <button class="btn-icon" onclick="app.showItemModal(${item.id})"><i class="fas fa-edit"></i></button>
          <button class="btn-icon" onclick="app.deleteItem(${item.id})"><i class="fas fa-trash"></i></button>
        </td>
      </tr>
    `).join('');
  }

  // Users Management
  async loadUsers() {
    if (this.user.role !== 'admin') {
      this.showToast('Access denied', 'error');
      return;
    }

    try {
      const users = await API.users.getAll();
      const tbody = document.getElementById('users-table-body');
      tbody.innerHTML = users.map(user => `
        <tr>
          <td>${user.username}</td>
          <td>${user.full_name || '-'}</td>
          <td>${user.role}</td>
          <td><span class="order-status ${user.active ? 'completed' : 'cancelled'}">${user.active ? 'Active' : 'Inactive'}</span></td>
          <td class="actions">
            <button class="btn-icon" onclick="app.showUserModal(${user.id})"><i class="fas fa-edit"></i></button>
            <button class="btn-icon" onclick="app.deleteUser(${user.id})"><i class="fas fa-trash"></i></button>
          </td>
        </tr>
      `).join('');
    } catch (error) {
      this.showToast('Failed to load users', 'error');
    }
  }

  // Modals
  showModal(title, body, footer = '') {
    document.getElementById('modal-title').textContent = title;
    document.getElementById('modal-body').innerHTML = body;
    document.getElementById('modal-footer').innerHTML = footer;
    document.getElementById('modal-overlay').classList.add('active');
  }

  closeModal() {
    document.getElementById('modal-overlay').classList.remove('active');
  }

  showLanguageModal() {
    const languages = i18n.getLanguages();
    const body = `
      <div class="payment-methods">
        ${languages.map(lang => `
          <div class="payment-method ${i18n.currentLang === lang.code ? 'active' : ''}" data-lang="${lang.code}">
            ${lang.name}
          </div>
        `).join('')}
      </div>
    `;

    this.showModal(i18n.t('language'), body);

    document.querySelectorAll('.payment-method[data-lang]').forEach(el => {
      el.addEventListener('click', () => {
        i18n.setLanguage(el.dataset.lang);
        this.closeModal();
        this.renderCategories();
      });
    });
  }

  showDiscountModal() {
    const body = `
      <div class="form-group">
        <label>${i18n.t('discount')} Type</label>
        <select id="discount-type">
          <option value="fixed">Fixed Amount</option>
          <option value="percentage">Percentage</option>
        </select>
      </div>
      <div class="form-group">
        <label>Amount</label>
        <input type="number" id="discount-amount" min="0" step="0.01" value="${this.discount.amount}">
      </div>
    `;

    const footer = `
      <button class="btn-secondary" onclick="app.closeModal()">${i18n.t('cancel')}</button>
      <button class="btn-primary" onclick="app.applyDiscount()">${i18n.t('save')}</button>
    `;

    this.showModal(i18n.t('discount'), body, footer);
  }

  applyDiscount() {
    this.discount = {
      type: document.getElementById('discount-type').value,
      amount: parseFloat(document.getElementById('discount-amount').value) || 0
    };
    this.updateCartTotals();
    this.closeModal();
  }

  showTableSelectModal() {
    this.loadTables().then(() => {
      const body = `
        <div class="table-select-grid">
          ${this.tables.map(table => `
            <div class="table-select-item ${table.status}" data-id="${table.id}" ${table.status === 'occupied' ? 'disabled' : ''}>
              <div class="name">${table.name}</div>
              <div class="capacity"><i class="fas fa-user"></i> ${table.capacity}</div>
            </div>
          `).join('')}
        </div>
      `;

      this.showModal(i18n.t('select_table'), body);

      document.querySelectorAll('.table-select-item:not(.occupied)').forEach(el => {
        el.addEventListener('click', () => {
          const tableId = parseInt(el.dataset.id);
          this.selectedTable = this.tables.find(t => t.id === tableId);
          document.getElementById('selected-table-name').textContent = this.selectedTable.name;
          document.getElementById('cart-table-info').style.display = 'flex';
          this.closeModal();
        });
      });
    });
  }

  showPaymentModal() {
    if (this.cart.length === 0) return;

    const total = this.getCartTotal();
    
    const body = `
      <div class="payment-total">
        <p>${i18n.t('total')}</p>
        <div class="amount">${this.currencySymbol}${total.toFixed(2)}</div>
      </div>
      <div class="payment-methods">
        <div class="payment-method active" data-method="cash">
          <i class="fas fa-money-bill-wave"></i>
          ${i18n.t('cash')}
        </div>
        <div class="payment-method" data-method="card">
          <i class="fas fa-credit-card"></i>
          ${i18n.t('card')}
        </div>
        <div class="payment-method" data-method="other">
          <i class="fas fa-wallet"></i>
          ${i18n.t('other')}
        </div>
      </div>
      <div class="form-group">
        <label>${i18n.t('amount_paid')}</label>
        <input type="number" id="payment-amount" min="${total}" step="0.01" value="${total.toFixed(2)}">
      </div>
      <div id="payment-change" style="display: none; text-align: center; font-size: 1.2rem; margin-top: 10px;">
        <strong>${i18n.t('change')}:</strong> <span id="change-amount">$0.00</span>
      </div>
    `;

    const footer = `
      <button class="btn-secondary" onclick="app.closeModal()">${i18n.t('cancel')}</button>
      <button class="btn-primary btn-lg" onclick="app.processPayment()">${i18n.t('complete_payment')}</button>
    `;

    this.showModal(i18n.t('payment'), body, footer);

    // Payment method selection
    document.querySelectorAll('.payment-method').forEach(el => {
      el.addEventListener('click', () => {
        document.querySelectorAll('.payment-method').forEach(m => m.classList.remove('active'));
        el.classList.add('active');
      });
    });

    // Calculate change
    document.getElementById('payment-amount').addEventListener('input', (e) => {
      const paid = parseFloat(e.target.value) || 0;
      const change = paid - total;
      const changeEl = document.getElementById('payment-change');
      const changeAmount = document.getElementById('change-amount');
      
      if (change > 0) {
        changeEl.style.display = 'block';
        changeAmount.textContent = `${this.currencySymbol}${change.toFixed(2)}`;
      } else {
        changeEl.style.display = 'none';
      }
    });
  }

  async processPayment() {
    const method = document.querySelector('.payment-method.active').dataset.method;
    const amount = parseFloat(document.getElementById('payment-amount').value);
    const total = this.getCartTotal();

    if (amount < total) {
      this.showToast('Insufficient payment amount', 'error');
      return;
    }

    try {
      // Create order
      const orderData = {
        table_id: this.selectedTable?.id,
        items: this.cart.map(item => ({
          id: item.id,
          name: item.name,
          price: item.price,
          tax_rate: item.tax_rate,
          quantity: item.quantity
        }))
      };

      const orderResult = await API.orders.create(orderData);

      // Process payment
      await API.orders.pay(orderResult.id, {
        method,
        amount
      });

      this.showToast(i18n.t('order_completed'), 'success');
      this.closeModal();
      
      // Clear cart
      this.clearCart();
      this.selectedTable = null;
      document.getElementById('cart-table-info').style.display = 'none';

      // Show success modal
      this.showOrderCompleteModal(orderResult.order_number, total, amount, method);

    } catch (error) {
      this.showToast(error.message || 'Payment failed', 'error');
    }
  }

  showOrderCompleteModal(orderNumber, total, paid, method) {
    const change = paid - total;
    
    const body = `
      <div style="text-align: center;">
        <i class="fas fa-check-circle" style="font-size: 4rem; color: var(--success); margin-bottom: 20px;"></i>
        <h2>${i18n.t('order_completed')}</h2>
        <p style="font-size: 1.2rem; margin: 10px 0;">${orderNumber}</p>
        <div style="margin: 20px 0; padding: 15px; background: var(--gray-100); border-radius: var(--radius);">
          <p><strong>${i18n.t('total')}:</strong> ${this.currencySymbol}${total.toFixed(2)}</p>
          <p><strong>${i18n.t('payment_method')}:</strong> ${method}</p>
          ${change > 0 ? `<p><strong>${i18n.t('change')}:</strong> ${this.currencySymbol}${change.toFixed(2)}</p>` : ''}
        </div>
      </div>
    `;

    const footer = `
      <button class="btn-secondary" onclick="window.print()"><i class="fas fa-print"></i> ${i18n.t('print_receipt')}</button>
      <button class="btn-primary" onclick="app.closeModal()">${i18n.t('new_order')}</button>
    `;

    this.showModal('', body, footer);
  }

  showOrderModal(order) {
    const body = `
      <div style="margin-bottom: 15px;">
        <strong>${i18n.t('order_number')}:</strong> ${order.order_number}<br>
        <strong>${i18n.t('date')}:</strong> ${new Date(order.created_at).toLocaleString()}<br>
        <strong>${i18n.t('table')}:</strong> ${order.table_name || '-'}<br>
        <strong>${i18n.t('status')}:</strong> <span class="order-status ${order.status}">${order.status}</span>
      </div>
      <table class="data-table">
        <thead>
          <tr>
            <th>${i18n.t('name')}</th>
            <th>Qty</th>
            <th>${i18n.t('price')}</th>
            <th>${i18n.t('total')}</th>
          </tr>
        </thead>
        <tbody>
          ${order.items.map(item => `
            <tr>
              <td>${item.item_name}</td>
              <td>${item.quantity}</td>
              <td>${this.currencySymbol}${item.unit_price.toFixed(2)}</td>
              <td>${this.currencySymbol}${item.total.toFixed(2)}</td>
            </tr>
          `).join('')}
        </tbody>
      </table>
      <div style="margin-top: 15px; text-align: right;">
        <p><strong>${i18n.t('subtotal')}:</strong> ${this.currencySymbol}${order.subtotal.toFixed(2)}</p>
        <p><strong>${i18n.t('tax')}:</strong> ${this.currencySymbol}${order.tax_amount.toFixed(2)}</p>
        ${order.discount_amount ? `<p><strong>${i18n.t('discount')}:</strong> -${this.currencySymbol}${order.discount_amount.toFixed(2)}</p>` : ''}
        <p style="font-size: 1.2rem;"><strong>${i18n.t('total')}:</strong> ${this.currencySymbol}${order.total.toFixed(2)}</p>
      </div>
    `;

    const footer = `
      <button class="btn-secondary" onclick="app.closeModal()">${i18n.t('cancel')}</button>
    `;

    this.showModal(`Order ${order.order_number}`, body, footer);
  }

  showTableModal(table = null) {
    const isEdit = !!table;
    
    const body = `
      <div class="form-group">
        <label>${i18n.t('name')}</label>
        <input type="text" id="table-name" value="${table?.name || ''}" required>
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>${i18n.t('capacity')}</label>
          <input type="number" id="table-capacity" min="1" value="${table?.capacity || 4}">
        </div>
        <div class="form-group">
          <label>${i18n.t('shape')}</label>
          <select id="table-shape">
            <option value="rectangle" ${table?.shape === 'rectangle' ? 'selected' : ''}>${i18n.t('rectangle')}</option>
            <option value="circle" ${table?.shape === 'circle' ? 'selected' : ''}>${i18n.t('circle')}</option>
          </select>
        </div>
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>Width</label>
          <input type="number" id="table-width" min="50" value="${table?.width || 100}">
        </div>
        <div class="form-group">
          <label>Height</label>
          <input type="number" id="table-height" min="50" value="${table?.height || 100}">
        </div>
      </div>
      ${isEdit ? `
        <div class="form-group">
          <label>${i18n.t('status')}</label>
          <select id="table-status">
            <option value="available" ${table?.status === 'available' ? 'selected' : ''}>${i18n.t('available')}</option>
            <option value="occupied" ${table?.status === 'occupied' ? 'selected' : ''}>${i18n.t('occupied')}</option>
            <option value="reserved" ${table?.status === 'reserved' ? 'selected' : ''}>${i18n.t('reserved')}</option>
            <option value="cleaning" ${table?.status === 'cleaning' ? 'selected' : ''}>${i18n.t('cleaning')}</option>
          </select>
        </div>
      ` : ''}
    `;

    const footer = `
      <button class="btn-secondary" onclick="app.closeModal()">${i18n.t('cancel')}</button>
      ${isEdit ? `<button class="btn-danger" onclick="app.deleteTable(${table.id})">${i18n.t('delete')}</button>` : ''}
      <button class="btn-primary" onclick="app.saveTable(${table?.id || 'null'})">${i18n.t('save')}</button>
    `;

    this.showModal(isEdit ? 'Edit Table' : i18n.t('add_table'), body, footer);
  }

  async saveTable(id) {
    const data = {
      name: document.getElementById('table-name').value,
      capacity: parseInt(document.getElementById('table-capacity').value),
      shape: document.getElementById('table-shape').value,
      width: parseInt(document.getElementById('table-width').value),
      height: parseInt(document.getElementById('table-height').value)
    };

    if (id) {
      data.status = document.getElementById('table-status').value;
    }

    try {
      if (id) {
        await API.tables.update(id, data);
      } else {
        await API.tables.create(data);
      }
      this.closeModal();
      this.loadTables();
      this.showToast('Table saved', 'success');
    } catch (error) {
      this.showToast(error.message || 'Failed to save table', 'error');
    }
  }

  async deleteTable(id) {
    if (!confirm('Delete this table?')) return;
    
    try {
      await API.tables.delete(id);
      this.closeModal();
      this.loadTables();
      this.showToast('Table deleted', 'success');
    } catch (error) {
      this.showToast(error.message || 'Failed to delete table', 'error');
    }
  }

  showItemModal(itemId = null) {
    const item = itemId ? this.items.find(i => i.id === itemId) : null;
    const isEdit = !!item;

    const body = `
      <div class="form-group">
        <label>${i18n.t('name')}</label>
        <input type="text" id="item-name" value="${item?.name || ''}" required>
      </div>
      <div class="form-group">
        <label>Name (Arabic)</label>
        <input type="text" id="item-name-ar" value="${item?.name_ar || ''}" dir="rtl">
      </div>
      <div class="form-group">
        <label>${i18n.t('category')}</label>
        <select id="item-category">
          <option value="">No Category</option>
          ${this.categories.map(cat => `
            <option value="${cat.id}" ${item?.category_id === cat.id ? 'selected' : ''}>${cat.name}</option>
          `).join('')}
        </select>
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>${i18n.t('price')}</label>
          <input type="number" id="item-price" min="0" step="0.01" value="${item?.price || ''}" required>
        </div>
        <div class="form-group">
          <label>${i18n.t('tax_rate')} (%)</label>
          <input type="number" id="item-tax" min="0" max="100" value="${(item?.tax_rate || 0) * 100}">
        </div>
      </div>
    `;

    const footer = `
      <button class="btn-secondary" onclick="app.closeModal()">${i18n.t('cancel')}</button>
      <button class="btn-primary" onclick="app.saveItem(${itemId || 'null'})">${i18n.t('save')}</button>
    `;

    this.showModal(isEdit ? 'Edit Item' : i18n.t('add_item'), body, footer);
  }

  async saveItem(id) {
    const data = {
      name: document.getElementById('item-name').value,
      name_ar: document.getElementById('item-name-ar').value,
      category_id: document.getElementById('item-category').value || null,
      price: parseFloat(document.getElementById('item-price').value),
      tax_rate: parseFloat(document.getElementById('item-tax').value) / 100
    };

    try {
      if (id) {
        await API.items.update(id, data);
      } else {
        await API.items.create(data);
      }
      this.closeModal();
      await this.loadItems();
      this.loadItemsManagement();
      this.showToast('Item saved', 'success');
    } catch (error) {
      this.showToast(error.message || 'Failed to save item', 'error');
    }
  }

  async deleteItem(id) {
    if (!confirm('Delete this item?')) return;
    
    try {
      await API.items.delete(id);
      await this.loadItems();
      this.loadItemsManagement();
      this.showToast('Item deleted', 'success');
    } catch (error) {
      this.showToast(error.message || 'Failed to delete item', 'error');
    }
  }

  showCategoryModal() {
    const body = `
      <div class="form-group">
        <label>${i18n.t('name')}</label>
        <input type="text" id="cat-name" required>
      </div>
      <div class="form-group">
        <label>Name (Arabic)</label>
        <input type="text" id="cat-name-ar" dir="rtl">
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>Color</label>
          <input type="color" id="cat-color" value="#3498db">
        </div>
        <div class="form-group">
          <label>Icon</label>
          <select id="cat-icon">
            <option value="utensils">Utensils</option>
            <option value="hamburger">Hamburger</option>
            <option value="pizza-slice">Pizza</option>
            <option value="coffee">Coffee</option>
            <option value="ice-cream">Ice Cream</option>
            <option value="wine-glass">Wine</option>
            <option value="beer">Beer</option>
            <option value="star">Star</option>
          </select>
        </div>
      </div>
    `;

    const footer = `
      <button class="btn-secondary" onclick="app.closeModal()">${i18n.t('cancel')}</button>
      <button class="btn-primary" onclick="app.saveCategory()">${i18n.t('save')}</button>
    `;

    this.showModal(i18n.t('add_category'), body, footer);
  }

  async saveCategory() {
    const data = {
      name: document.getElementById('cat-name').value,
      name_ar: document.getElementById('cat-name-ar').value,
      color: document.getElementById('cat-color').value,
      icon: document.getElementById('cat-icon').value
    };

    try {
      await API.items.createCategory(data);
      this.closeModal();
      await this.loadCategories();
      this.loadItemsManagement();
      this.showToast('Category saved', 'success');
    } catch (error) {
      this.showToast(error.message || 'Failed to save category', 'error');
    }
  }

  showUserModal(userId = null) {
    const body = `
      <div class="form-group">
        <label>${i18n.t('username')}</label>
        <input type="text" id="user-username" required>
      </div>
      <div class="form-group">
        <label>${i18n.t('password')} ${userId ? '(leave empty to keep current)' : ''}</label>
        <input type="password" id="user-password" ${userId ? '' : 'required'}>
      </div>
      <div class="form-group">
        <label>${i18n.t('full_name')}</label>
        <input type="text" id="user-fullname">
      </div>
      <div class="form-group">
        <label>${i18n.t('role')}</label>
        <select id="user-role">
          <option value="cashier">${i18n.t('cashier')}</option>
          <option value="manager">${i18n.t('manager')}</option>
          <option value="admin">${i18n.t('admin')}</option>
        </select>
      </div>
    `;

    const footer = `
      <button class="btn-secondary" onclick="app.closeModal()">${i18n.t('cancel')}</button>
      <button class="btn-primary" onclick="app.saveUser(${userId || 'null'})">${i18n.t('save')}</button>
    `;

    this.showModal(userId ? 'Edit User' : i18n.t('add_user'), body, footer);
  }

  async saveUser(id) {
    const data = {
      username: document.getElementById('user-username').value,
      full_name: document.getElementById('user-fullname').value,
      role: document.getElementById('user-role').value
    };

    const password = document.getElementById('user-password').value;
    if (password) {
      data.password = password;
    }

    try {
      if (id) {
        await API.users.update(id, data);
      } else {
        await API.users.create(data);
      }
      this.closeModal();
      this.loadUsers();
      this.showToast('User saved', 'success');
    } catch (error) {
      this.showToast(error.message || 'Failed to save user', 'error');
    }
  }

  async deleteUser(id) {
    if (!confirm('Delete this user?')) return;
    
    try {
      await API.users.delete(id);
      this.loadUsers();
      this.showToast('User deleted', 'success');
    } catch (error) {
      this.showToast(error.message || 'Failed to delete user', 'error');
    }
  }

  // Toast Notifications
  showToast(message, type = 'info') {
    const container = document.getElementById('toast-container');
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    
    const icons = {
      success: 'check-circle',
      error: 'exclamation-circle',
      warning: 'exclamation-triangle',
      info: 'info-circle'
    };

    toast.innerHTML = `
      <i class="fas fa-${icons[type]}"></i>
      <span>${message}</span>
    `;

    container.appendChild(toast);

    setTimeout(() => {
      toast.style.animation = 'slideIn 0.3s ease reverse';
      setTimeout(() => toast.remove(), 300);
    }, 3000);
  }
}

// Initialize app
let app;
document.addEventListener('DOMContentLoaded', () => {
  app = new SmartPOS();
});
