/**
 * Ierahkwa Futurehead Shop - Admin Panel
 * Multi-Purpose E-Commerce Management
 */
(function() {
  'use strict';
  
  let token = localStorage.getItem('ierahkwa_admin_token');
  let currentUser = null;
  
  function auth() {
    return { 
      headers: { 
        'Authorization': 'Bearer ' + token, 
        'X-Admin-Key': token,
        'Content-Type': 'application/json'
      } 
    };
  }

  function api(url, opts) {
    opts = opts || {};
    opts.headers = Object.assign({}, auth().headers, opts.headers || {});
    return fetch(url, opts).then(function(r) {
      if (r.status === 401) {
        token = null;
        localStorage.removeItem('ierahkwa_admin_token');
        showPage('login');
        return Promise.reject('Unauthorized');
      }
      return r.json().catch(function() { return {}; });
    });
  }

  function apiGet(url) { return api(url); }
  function apiPost(url, data) { return api(url, { method: 'POST', body: JSON.stringify(data) }); }
  function apiPut(url, data) { return api(url, { method: 'PUT', body: JSON.stringify(data) }); }
  function apiPatch(url, data) { return api(url, { method: 'PATCH', body: JSON.stringify(data) }); }
  function apiDelete(url) { return api(url, { method: 'DELETE' }); }

  // ═══════════════════════════════════════════════════════════════
  // NAVIGATION
  // ═══════════════════════════════════════════════════════════════
  
  function showPage(name) {
    document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
    const el = document.getElementById('page-' + name);
    if (el) el.classList.add('active');
    
    document.querySelectorAll('.sidebar .nav-link').forEach(n => {
      n.classList.toggle('active', n.dataset.page === name);
    });
    
    if (name !== 'login') {
      $('#nav-auth').removeClass('d-none');
      loadPageData(name);
    }
  }

  document.querySelectorAll('[data-page]').forEach(function(a) {
    a.addEventListener('click', function(e) {
      e.preventDefault();
      const p = this.dataset.page;
      if (p) {
        location.hash = p;
        showPage(p);
      }
    });
  });

  window.addEventListener('hashchange', function() {
    const h = (location.hash || '#login').replace(/^#/, '');
    if (token && h !== 'login') showPage(h);
  });

  // ═══════════════════════════════════════════════════════════════
  // AUTH
  // ═══════════════════════════════════════════════════════════════
  
  $('#login-btn').on('click', function() {
    const email = $('#login-email').val();
    const pass = $('#login-pass').val();
    
    $(this).prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span>');
    $('#login-error').text('');
    
    fetch('/api/admin/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body: 'email=' + encodeURIComponent(email) + '&password=' + encodeURIComponent(pass)
    })
    .then(r => r.json())
    .then(function(d) {
      if (d.token) {
        token = d.token;
        currentUser = d.user;
        localStorage.setItem('ierahkwa_admin_token', token);
        location.hash = 'dashboard';
        showPage('dashboard');
      } else {
        $('#login-error').text(d.error || 'Login failed');
      }
    })
    .catch(function() {
      $('#login-error').text('Connection error');
    })
    .finally(function() {
      $('#login-btn').prop('disabled', false).html('<i class="bi bi-box-arrow-in-right me-2"></i>Login');
    });
  });

  $('#logout').on('click', function(e) {
    e.preventDefault();
    token = null;
    currentUser = null;
    localStorage.removeItem('ierahkwa_admin_token');
    $('#nav-auth').addClass('d-none');
    location.hash = 'login';
    showPage('login');
  });

  // ═══════════════════════════════════════════════════════════════
  // PAGE DATA LOADERS
  // ═══════════════════════════════════════════════════════════════
  
  function loadPageData(name) {
    switch(name) {
      case 'dashboard': loadDashboard(); break;
      case 'products': loadProducts(); break;
      case 'categories': loadCategories(); break;
      case 'suppliers': loadSuppliers(); break;
      case 'orders': loadOrders(); break;
      case 'customers': loadCustomers(); break;
      case 'reports': loadReports(); break;
      case 'inventory': loadInventory(); break;
      case 'settings': loadSettings(); break;
      case 'users': loadUsers(); break;
      case 'activity': loadActivity(); break;
    }
  }

  // ═══════════════════════════════════════════════════════════════
  // DASHBOARD
  // ═══════════════════════════════════════════════════════════════
  
  function loadDashboard() {
    apiGet('/api/admin/dashboard').then(function(d) {
      const s = d.stats || {};
      $('#stat-today-orders').text(s.today_orders || 0);
      $('#stat-today-sales').text('$' + (s.today_sales || 0).toFixed(2));
      $('#stat-month-orders').text(s.month_orders || 0);
      $('#stat-month-sales').text('$' + (s.month_sales || 0).toFixed(2));
      $('#stat-products').text(s.total_products || 0);
      $('#stat-customers').text(s.total_customers || 0);
      $('#stat-pending').text(s.pending_orders || 0);
      $('#stat-lowstock').text(s.low_stock_count || 0);
      
      // Recent orders
      let html = '';
      (d.recent_orders || []).forEach(function(o) {
        html += `<tr>
          <td><a href="#orders" class="text-warning">${o.order_number}</a></td>
          <td>$${(o.total || 0).toFixed(2)}</td>
          <td><span class="badge badge-status badge-${o.status}">${o.status}</span></td>
        </tr>`;
      });
      $('#recent-orders').html(html || '<tr><td colspan="3" class="text-muted">No recent orders</td></tr>');
      
      // Low stock
      html = '';
      (d.low_stock_products || []).forEach(function(p) {
        html += `<tr><td>${p.name}</td><td class="text-warning">${p.stock}</td><td>${p.min_stock || 10}</td></tr>`;
      });
      $('#lowstock-products').html(html || '<tr><td colspan="3" class="text-muted">No low stock products</td></tr>');
    });
  }

  // ═══════════════════════════════════════════════════════════════
  // PRODUCTS
  // ═══════════════════════════════════════════════════════════════
  
  function loadProducts() {
    apiGet('/api/admin/products').then(function(r) {
      let html = '';
      (r.data || []).forEach(function(p) {
        const status = p.is_active ? '<span class="badge bg-success">Active</span>' : '<span class="badge bg-secondary">Inactive</span>';
        const stockClass = p.stock <= (p.min_stock || 10) ? 'text-warning' : '';
        html += `<tr>
          <td>${p.id}</td>
          <td><strong>${p.name}</strong>${p.variant_count ? ` <small class="text-muted">(${p.variant_count} variants)</small>` : ''}</td>
          <td><code>${p.sku || '-'}</code></td>
          <td>${p.category_name || '-'}</td>
          <td>$${(p.price || 0).toFixed(2)}</td>
          <td class="${stockClass}">${p.stock}</td>
          <td>${status}</td>
          <td>
            <button class="btn btn-sm btn-outline-warning edit-product" data-id="${p.id}"><i class="bi bi-pencil"></i></button>
            <button class="btn btn-sm btn-outline-danger delete-product" data-id="${p.id}" data-name="${p.name}"><i class="bi bi-trash"></i></button>
          </td>
        </tr>`;
      });
      $('#products-table').html(html || '<tr><td colspan="8" class="text-muted">No products found</td></tr>');
    });
  }

  $(document).on('click', '.delete-product', function() {
    const id = $(this).data('id');
    const name = $(this).data('name');
    if (confirm(`Delete product "${name}"?`)) {
      apiDelete('/api/admin/products/' + id).then(function() {
        loadProducts();
      });
    }
  });

  // ═══════════════════════════════════════════════════════════════
  // CATEGORIES
  // ═══════════════════════════════════════════════════════════════
  
  function loadCategories() {
    apiGet('/api/admin/categories').then(function(r) {
      const cats = r.data || [];
      let html = '';
      cats.forEach(function(c) {
        const parent = cats.find(x => x.id === c.parent_id);
        html += `<tr>
          <td>${c.id}</td>
          <td>${c.name}</td>
          <td><code>${c.slug}</code></td>
          <td>${parent ? parent.name : '-'}</td>
          <td>${c.is_featured ? '<i class="bi bi-star-fill text-warning"></i>' : ''}</td>
          <td>${c.is_active ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
        </tr>`;
      });
      $('#categories-table').html(html || '<tr><td colspan="6" class="text-muted">No categories</td></tr>');
    });
  }

  // ═══════════════════════════════════════════════════════════════
  // SUPPLIERS
  // ═══════════════════════════════════════════════════════════════
  
  function loadSuppliers() {
    apiGet('/api/admin/suppliers').then(function(r) {
      let html = '';
      (r.data || []).forEach(function(s) {
        html += `<tr>
          <td>${s.id}</td>
          <td>${s.name}</td>
          <td><code>${s.code || '-'}</code></td>
          <td>${s.email || '-'}</td>
          <td>${s.phone || '-'}</td>
          <td>${s.is_active ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
        </tr>`;
      });
      $('#suppliers-table').html(html || '<tr><td colspan="6" class="text-muted">No suppliers</td></tr>');
    });
  }

  // ═══════════════════════════════════════════════════════════════
  // ORDERS
  // ═══════════════════════════════════════════════════════════════
  
  function loadOrders() {
    const params = new URLSearchParams();
    const status = $('#orders-status').val();
    const from = $('#orders-from').val();
    const to = $('#orders-to').val();
    const q = $('#orders-search').val();
    
    if (status) params.set('status', status);
    if (from) params.set('from', from);
    if (to) params.set('to', to);
    if (q) params.set('q', q);
    
    apiGet('/api/admin/orders?' + params).then(function(r) {
      let html = '';
      (r.data || []).forEach(function(o) {
        const date = (o.created_at || '').slice(0, 10);
        html += `<tr>
          <td><strong>${o.order_number}</strong></td>
          <td>${o.customer_name || o.guest_name || o.guest_email || 'Guest'}</td>
          <td>${o.item_count || '-'}</td>
          <td>$${(o.total || 0).toFixed(2)}</td>
          <td><span class="badge badge-status badge-${o.status}">${o.status}</span></td>
          <td><span class="badge ${o.payment_status === 'paid' ? 'bg-success' : 'bg-warning text-dark'}">${o.payment_status}</span></td>
          <td>${date}</td>
          <td>
            <button class="btn btn-sm btn-outline-info view-order" data-id="${o.id}"><i class="bi bi-eye"></i></button>
            <a class="btn btn-sm btn-outline-warning" href="/invoice.html?order=${o.order_number}" target="_blank"><i class="bi bi-printer"></i></a>
          </td>
        </tr>`;
      });
      $('#orders-table').html(html || '<tr><td colspan="8" class="text-muted">No orders found</td></tr>');
    });
  }

  $('#orders-filter').on('click', loadOrders);

  // ═══════════════════════════════════════════════════════════════
  // CUSTOMERS
  // ═══════════════════════════════════════════════════════════════
  
  function loadCustomers() {
    apiGet('/api/admin/customers').then(function(r) {
      let html = '';
      (r.data || []).forEach(function(c) {
        html += `<tr>
          <td>${c.id}</td>
          <td>${c.name}</td>
          <td>${c.email || '-'}</td>
          <td>${c.phone || '-'}</td>
          <td>${c.total_orders || 0}</td>
          <td>$${(c.total_spent || 0).toFixed(2)}</td>
        </tr>`;
      });
      $('#customers-table').html(html || '<tr><td colspan="6" class="text-muted">No customers</td></tr>');
    });
  }

  // ═══════════════════════════════════════════════════════════════
  // REPORTS
  // ═══════════════════════════════════════════════════════════════
  
  function loadReports() {
    // Set default dates
    const today = new Date().toISOString().slice(0, 10);
    const monthAgo = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().slice(0, 10);
    if (!$('#report-from').val()) $('#report-from').val(monthAgo);
    if (!$('#report-to').val()) $('#report-to').val(today);
    
    generateReport();
  }

  function generateReport() {
    const params = new URLSearchParams();
    params.set('from', $('#report-from').val());
    params.set('to', $('#report-to').val());
    params.set('group', $('#report-group').val());
    
    apiGet('/api/admin/reports/sales?' + params).then(function(r) {
      const s = r.summary || {};
      $('#report-total-orders').text(s.total_orders || 0);
      $('#report-total-sales').text('$' + (s.total_sales || 0).toFixed(2));
      $('#report-avg-order').text('$' + (s.avg_order || 0).toFixed(2));
      
      let html = '';
      (r.data || []).forEach(function(d) {
        html += `<tr>
          <td>${d.period}</td>
          <td>${d.orders}</td>
          <td>$${(d.subtotal || 0).toFixed(2)}</td>
          <td>$${(d.discount || 0).toFixed(2)}</td>
          <td>$${(d.shipping || 0).toFixed(2)}</td>
          <td class="text-warning">$${(d.total || 0).toFixed(2)}</td>
        </tr>`;
      });
      $('#report-table').html(html || '<tr><td colspan="6" class="text-muted">No data</td></tr>');
    });
  }

  $('#report-generate').on('click', generateReport);

  // ═══════════════════════════════════════════════════════════════
  // INVENTORY
  // ═══════════════════════════════════════════════════════════════
  
  function loadInventory() {
    apiGet('/api/admin/reports/inventory').then(function(r) {
      const s = r.summary || {};
      $('#inv-products').text(s.total_products || 0);
      $('#inv-stock').text(s.total_stock || 0);
      $('#inv-lowstock').text(s.low_stock_count || 0);
      $('#inv-outstock').text(s.out_of_stock_count || 0);
      
      let html = '';
      (r.low_stock || []).forEach(function(p) {
        html += `<tr><td>${p.name}</td><td><code>${p.sku}</code></td><td class="text-warning">${p.stock}</td><td>${p.min_stock || 10}</td></tr>`;
      });
      $('#inv-lowstock-table').html(html || '<tr><td colspan="4" class="text-muted">None</td></tr>');
      
      html = '';
      (r.out_of_stock || []).forEach(function(p) {
        html += `<tr><td>${p.name}</td><td><code>${p.sku}</code></td></tr>`;
      });
      $('#inv-outstock-table').html(html || '<tr><td colspan="2" class="text-muted">None</td></tr>');
    });
  }

  // ═══════════════════════════════════════════════════════════════
  // SETTINGS
  // ═══════════════════════════════════════════════════════════════
  
  function loadSettings() {
    apiGet('/api/admin/settings').then(function(s) {
      $('#settings-form [name]').each(function() {
        const name = $(this).attr('name');
        if (s[name] !== undefined) $(this).val(s[name]);
      });
    });
  }

  $('#settings-form').on('submit', function(e) {
    e.preventDefault();
    const data = {};
    $(this).find('[name]').each(function() {
      data[$(this).attr('name')] = $(this).val();
    });
    
    apiPost('/api/admin/settings', data).then(function() {
      alert('Settings saved!');
    });
  });

  // ═══════════════════════════════════════════════════════════════
  // USERS
  // ═══════════════════════════════════════════════════════════════
  
  function loadUsers() {
    apiGet('/api/admin/users').then(function(r) {
      let html = '';
      (r.data || []).forEach(function(u) {
        const lastLogin = u.last_login ? u.last_login.slice(0, 16).replace('T', ' ') : 'Never';
        html += `<tr>
          <td>${u.id}</td>
          <td>${u.name || '-'}</td>
          <td>${u.email}</td>
          <td>${u.role_name || '-'}</td>
          <td>${u.branch_name || '-'}</td>
          <td>${lastLogin}</td>
          <td>${u.is_active ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
        </tr>`;
      });
      $('#users-table').html(html || '<tr><td colspan="7" class="text-muted">No users</td></tr>');
    });
  }

  // ═══════════════════════════════════════════════════════════════
  // ACTIVITY
  // ═══════════════════════════════════════════════════════════════
  
  function loadActivity() {
    apiGet('/api/admin/activity?limit=100').then(function(r) {
      let html = '';
      (r.data || []).forEach(function(a) {
        const time = (a.created_at || '').slice(0, 16).replace('T', ' ');
        html += `<tr>
          <td class="small">${time}</td>
          <td>${a.user_id || 'System'}</td>
          <td>${a.action}</td>
          <td>${a.entity_type || '-'} ${a.entity_id || ''}</td>
          <td class="small text-muted">${a.details || ''}</td>
        </tr>`;
      });
      $('#activity-table').html(html || '<tr><td colspan="5" class="text-muted">No activity</td></tr>');
    });
  }

  // ═══════════════════════════════════════════════════════════════
  // INIT
  // ═══════════════════════════════════════════════════════════════
  
  if (token) {
    const hash = (location.hash || '#dashboard').replace(/^#/, '');
    $('#nav-auth').removeClass('d-none');
    showPage(hash || 'dashboard');
  } else {
    showPage('login');
  }
})();
