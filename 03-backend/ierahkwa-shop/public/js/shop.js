/**
 * Ierahkwa Futurehead Shop - Multi-Purpose E-Commerce
 * Frontend JavaScript
 */
(function() {
  'use strict';
  
  const CART_KEY = 'ierahkwa_cart';
  const CURRENCY = '$';
  let lang = localStorage.getItem('ierahkwa_lang') || 'en';
  let t = {};
  let settings = {};
  let shippingMethods = [];
  let paymentMethods = [];
  let selectedShipping = null;
  let appliedCoupon = null;
  let couponDiscount = 0;

  // ═══════════════════════════════════════════════════════════════
  // LOCALIZATION
  // ═══════════════════════════════════════════════════════════════
  
  function loadLocale(l) {
    return $.getJSON('/locales/' + l + '.json').then(function(r) { 
      t = r; 
      return r; 
    }).catch(function() {
      return {};
    });
  }

  function i18n(k) { return t[k] || k; }

  function applyLang() {
    document.querySelectorAll('[data-i18n]').forEach(function(el) { 
      el.textContent = i18n(el.getAttribute('data-i18n')); 
    });
    $('#search').attr('placeholder', t.search_placeholder || 'Search products...');
    document.querySelectorAll('.lang-btn').forEach(function(b) { 
      b.classList.toggle('active', b.dataset.lang === lang); 
    });
    localStorage.setItem('ierahkwa_lang', lang);
  }

  $(document).on('click', '.lang-btn', function() {
    lang = this.dataset.lang;
    loadLocale(lang).then(applyLang);
  });

  // ═══════════════════════════════════════════════════════════════
  // CART
  // ═══════════════════════════════════════════════════════════════
  
  function getCart() { 
    try { return JSON.parse(localStorage.getItem(CART_KEY) || '[]'); } 
    catch(e) { return []; } 
  }
  
  function setCart(c) { 
    localStorage.setItem(CART_KEY, JSON.stringify(c)); 
    renderCart(); 
  }

  function addToCart(product, variant, qty) {
    const c = getCart();
    const key = variant ? `${product.id}-${variant.id}` : `${product.id}`;
    const i = c.findIndex(x => x.key === key);
    const price = variant ? variant.price : (product.final_price || product.price);
    
    qty = Math.max(1, parseInt(qty, 10) || 1);
    
    if (i >= 0) {
      c[i].qty += qty;
    } else {
      c.push({ 
        key,
        product_id: product.id, 
        variant_id: variant?.id || null,
        name: product.name,
        variant_name: variant ? Object.values(variant.attributes || {}).join(' / ') : null,
        price,
        qty,
        sku: variant?.sku || product.sku,
        barcode: variant?.barcode || product.barcode,
        image: (product.images && product.images[0]) || null
      });
    }
    setCart(c);
    
    // Show cart
    bootstrap.Offcanvas.getOrCreateInstance(document.getElementById('cartOffcanvas')).show();
  }

  function updateCartQty(key, qty) {
    const c = getCart();
    const i = c.findIndex(x => x.key === key);
    if (i >= 0) {
      if (qty <= 0) {
        c.splice(i, 1);
      } else {
        c[i].qty = qty;
      }
      setCart(c);
    }
  }

  function removeFromCart(key) {
    setCart(getCart().filter(x => x.key !== key));
  }

  function cartCount() { 
    return getCart().reduce((s, x) => s + (x.qty || 0), 0); 
  }
  
  function cartSubtotal() {
    return getCart().reduce((s, x) => s + (x.price || 0) * (x.qty || 0), 0);
  }

  function renderCart() {
    const c = getCart();
    $('#cart-count').text(cartCount());
    
    if (c.length === 0) {
      $('#cart-items').html('<div class="text-center text-muted py-4"><i class="bi bi-cart-x" style="font-size:3rem"></i><p class="mt-2">Your cart is empty</p></div>');
      $('#checkout-btn').prop('disabled', true);
    } else {
      let html = '';
      c.forEach(function(x) {
        const total = (x.price * x.qty).toFixed(2);
        html += `
          <div class="d-flex align-items-center border-bottom border-secondary pb-2 mb-2">
            <div class="flex-grow-1">
              <div class="fw-semibold">${x.name}</div>
              ${x.variant_name ? `<small class="text-muted">${x.variant_name}</small>` : ''}
              <div class="small text-warning">${CURRENCY}${x.price.toFixed(2)} × ${x.qty}</div>
            </div>
            <div class="text-end">
              <div class="fw-bold">${CURRENCY}${total}</div>
              <div class="btn-group btn-group-sm mt-1">
                <button class="btn btn-outline-secondary btn-sm cart-qty" data-key="${x.key}" data-action="dec">-</button>
                <button class="btn btn-outline-secondary btn-sm" disabled>${x.qty}</button>
                <button class="btn btn-outline-secondary btn-sm cart-qty" data-key="${x.key}" data-action="inc">+</button>
                <button class="btn btn-outline-danger btn-sm cart-remove" data-key="${x.key}"><i class="bi bi-trash"></i></button>
              </div>
            </div>
          </div>`;
      });
      $('#cart-items').html(html);
      $('#checkout-btn').prop('disabled', false);
    }
    
    const sub = cartSubtotal();
    $('#cart-sub').text(CURRENCY + sub.toFixed(2));
    $('#cart-total').text(CURRENCY + sub.toFixed(2));
    updateCheckoutTotal();
  }

  $(document).on('click', '.cart-qty', function() {
    const key = $(this).data('key');
    const action = $(this).data('action');
    const c = getCart();
    const item = c.find(x => x.key === key);
    if (item) {
      updateCartQty(key, action === 'inc' ? item.qty + 1 : item.qty - 1);
    }
  });

  $(document).on('click', '.cart-remove', function() {
    removeFromCart($(this).data('key'));
  });

  // ═══════════════════════════════════════════════════════════════
  // PRODUCTS
  // ═══════════════════════════════════════════════════════════════
  
  let currentPage = 1;
  
  function loadProducts(page) {
    page = page || 1;
    currentPage = page;
    
    const params = new URLSearchParams();
    const q = $('#search').val();
    const cat = $('#filter-cat').val();
    const brand = $('#filter-brand').val();
    const color = $('#filter-color').val();
    const size = $('#filter-size').val();
    const minPrice = $('#filter-min').val();
    const maxPrice = $('#filter-max').val();
    const sort = $('#filter-sort').val();
    
    if (q) params.set('q', q);
    if (cat) params.set('category', cat);
    if (brand) params.set('brand', brand);
    if (color) params.set('color', color);
    if (size) params.set('size', size);
    if (minPrice) params.set('min_price', minPrice);
    if (maxPrice) params.set('max_price', maxPrice);
    if (sort) params.set('sort', sort);
    params.set('page', page);
    params.set('limit', 24);
    
    $('#loading').removeClass('d-none');
    $('#empty').addClass('d-none');
    $('#products').html('');
    
    $.get('/api/products?' + params).then(function(r) {
      $('#loading').addClass('d-none');
      const list = r.data || [];
      $('#product-count').text(r.total || 0);
      
      if (list.length === 0) {
        $('#empty').removeClass('d-none');
        $('#pagination').html('');
        return;
      }
      
      let html = '';
      list.forEach(function(p) {
        const finalPrice = p.final_price || p.price;
        const hasDiscount = p.compare_price && p.compare_price > finalPrice;
        const badges = [];
        if (p.is_new) badges.push('<span class="badge badge-new">New</span>');
        if (p.discount_percent > 0) badges.push(`<span class="badge badge-sale">-${p.discount_percent}%</span>`);
        
        html += `
          <div class="col">
            <div class="card h-100">
              <div class="card-img-top">
                ${p.images && p.images[0] ? `<img src="${p.images[0]}" class="img-fluid" alt="${p.name}">` : '<i class="bi bi-image"></i>'}
              </div>
              <div class="card-body">
                ${badges.length ? `<div class="mb-1">${badges.join(' ')}</div>` : ''}
                <span class="category-badge">${p.category_name || 'General'}</span>
                <h6 class="card-title mt-2 text-truncate" title="${p.name}">${p.name}</h6>
                <div class="d-flex align-items-baseline gap-2">
                  <span class="price">${CURRENCY}${finalPrice.toFixed(2)}</span>
                  ${hasDiscount ? `<span class="price-old">${CURRENCY}${p.compare_price.toFixed(2)}</span>` : ''}
                </div>
                ${p.attributes && (p.attributes.color || p.attributes.size) ? `<div class="product-attrs mt-1">${[p.attributes.color, p.attributes.size].filter(Boolean).join(' / ')}</div>` : ''}
              </div>
              <div class="card-footer bg-transparent border-0 pt-0">
                <button class="btn btn-ierahkwa btn-sm w-100 add-cart" 
                  data-id="${p.id}" 
                  data-name="${p.name}" 
                  data-price="${finalPrice}" 
                  data-sku="${p.sku || ''}" 
                  data-barcode="${p.barcode || ''}"
                  data-has-variants="${p.has_variants || p.type === 'variable' ? '1' : '0'}"
                  data-slug="${p.slug}">
                  <i class="bi bi-cart-plus me-1"></i>${i18n('add_to_cart') || 'Add to Cart'}
                </button>
              </div>
            </div>
          </div>`;
      });
      $('#products').html(html);
      
      // Pagination
      if (r.pages > 1) {
        let phtml = '<ul class="pagination pagination-sm">';
        if (page > 1) phtml += `<li class="page-item"><a class="page-link bg-dark text-light border-secondary" href="#" data-page="${page - 1}">«</a></li>`;
        for (let i = 1; i <= Math.min(r.pages, 5); i++) {
          const active = i === page ? 'active' : '';
          phtml += `<li class="page-item ${active}"><a class="page-link bg-dark text-light border-secondary" href="#" data-page="${i}">${i}</a></li>`;
        }
        if (page < r.pages) phtml += `<li class="page-item"><a class="page-link bg-dark text-light border-secondary" href="#" data-page="${page + 1}">»</a></li>`;
        phtml += '</ul>';
        $('#pagination').html(phtml);
      } else {
        $('#pagination').html('');
      }
    }).fail(function() {
      $('#loading').addClass('d-none');
      $('#empty').removeClass('d-none').find('p').text('Error loading products');
    });
  }

  $(document).on('click', '#pagination a', function(e) {
    e.preventDefault();
    loadProducts(parseInt($(this).data('page'), 10));
    window.scrollTo(0, 0);
  });

  // Add to cart
  $(document).on('click', '.add-cart', function() {
    const btn = $(this);
    const hasVariants = btn.data('has-variants') === '1' || btn.data('has-variants') === 1;
    
    if (hasVariants) {
      // TODO: Show variant selector modal
      // For now, just add base product
    }
    
    addToCart({
      id: parseInt(btn.data('id'), 10),
      name: btn.data('name'),
      price: parseFloat(btn.data('price')),
      final_price: parseFloat(btn.data('price')),
      sku: btn.data('sku'),
      barcode: btn.data('barcode'),
      slug: btn.data('slug')
    }, null, 1);
  });

  // Filters
  let searchTimeout;
  $('#search').on('input', function() {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(function() { loadProducts(1); }, 300);
  });
  
  $('#filter-cat, #filter-brand, #filter-color, #filter-size, #filter-sort').on('change', function() {
    loadProducts(1);
  });
  
  $('#filter-min, #filter-max').on('change', function() {
    loadProducts(1);
  });
  
  $('#clear-filters').on('click', function() {
    $('#filter-cat, #filter-brand, #filter-color, #filter-size, #filter-sort').val('');
    $('#filter-min, #filter-max').val('');
    $('#search').val('');
    loadProducts(1);
  });

  // Barcode scan
  $('#scan-btn').on('click', function() {
    $('#search').focus().attr('placeholder', 'Scan barcode and press Enter...');
  });
  
  $('#search').on('keypress', function(e) {
    if (e.which !== 13) return;
    e.preventDefault();
    const v = $(this).val().trim();
    if (v.length >= 4) {
      $.get('/api/products/barcode/' + encodeURIComponent(v)).then(function(p) {
        const price = p.variant ? p.variant.price : (p.final_price || p.price);
        addToCart(p, p.variant, 1);
        $('#search').val('');
      }).fail(function() {
        loadProducts(1);
      });
    } else {
      loadProducts(1);
    }
  });

  // ═══════════════════════════════════════════════════════════════
  // FILTERS INIT
  // ═══════════════════════════════════════════════════════════════
  
  function loadFilters() {
    $.get('/api/categories').then(function(r) {
      const cats = r.flat || r.data || [];
      cats.forEach(function(c) {
        const indent = c.parent_id ? '— ' : '';
        $('#filter-cat').append(`<option value="${c.id}">${indent}${c.name}</option>`);
      });
    });
    
    $.get('/api/filters').then(function(r) {
      (r.colors || []).forEach(function(c) {
        $('#filter-color').append(`<option value="${c}">${c}</option>`);
      });
      (r.sizes || []).forEach(function(s) {
        $('#filter-size').append(`<option value="${s}">${s}</option>`);
      });
      (r.brands || []).forEach(function(b) {
        $('#filter-brand').append(`<option value="${b.id}">${b.name}</option>`);
      });
    });
  }

  // ═══════════════════════════════════════════════════════════════
  // CHECKOUT
  // ═══════════════════════════════════════════════════════════════
  
  function loadCheckoutOptions() {
    $.get('/api/shipping-methods').then(function(r) {
      shippingMethods = r.data || [];
      let html = '';
      shippingMethods.forEach(function(s, i) {
        const checked = i === 0 ? 'checked' : '';
        html += `
          <div class="form-check">
            <input class="form-check-input" type="radio" name="shipping" id="ship${s.id}" value="${s.id}" ${checked}>
            <label class="form-check-label small" for="ship${s.id}">
              ${s.name} - ${s.price > 0 ? CURRENCY + s.price.toFixed(2) : 'Free'}
              <span class="text-muted">(${s.description})</span>
            </label>
          </div>`;
      });
      $('#shipping-methods').html(html || '<p class="text-muted small">No shipping methods available</p>');
      if (shippingMethods.length) selectedShipping = shippingMethods[0];
      updateCheckoutTotal();
    });
    
    $.get('/api/payment-methods').then(function(r) {
      paymentMethods = r.data || [];
      let html = '';
      paymentMethods.forEach(function(p, i) {
        const checked = i === 0 ? 'checked' : '';
        html += `
          <div class="form-check">
            <input class="form-check-input" type="radio" name="payment" id="pay${p.code}" value="${p.code}" ${checked}>
            <label class="form-check-label small" for="pay${p.code}">
              <i class="bi ${p.icon} me-1"></i>${p.name}
            </label>
          </div>`;
      });
      $('#payment-methods').html(html || '<p class="text-muted small">No payment methods available</p>');
    });
  }

  $(document).on('change', 'input[name="shipping"]', function() {
    const id = parseInt($(this).val(), 10);
    selectedShipping = shippingMethods.find(s => s.id === id);
    updateCheckoutTotal();
  });

  function updateCheckoutTotal() {
    const sub = cartSubtotal();
    let shipping = selectedShipping ? selectedShipping.price : 0;
    
    // Free shipping check
    const freeMin = parseFloat(settings.free_shipping_min || 0);
    if (freeMin > 0 && sub >= freeMin) shipping = 0;
    if (appliedCoupon && appliedCoupon.type === 'free_shipping') shipping = 0;
    
    const total = sub - couponDiscount + shipping;
    $('#checkout-total').text(CURRENCY + total.toFixed(2));
  }

  $('#apply-coupon').on('click', function() {
    const code = $('#co-coupon').val().trim();
    if (!code) return;
    
    const sub = cartSubtotal();
    $.post('/api/coupons/validate', { code, subtotal: sub }).then(function(r) {
      appliedCoupon = r.coupon;
      couponDiscount = r.discount || 0;
      $('#coupon-result').html(`<span class="text-success"><i class="bi bi-check-circle me-1"></i>Coupon applied: -${CURRENCY}${couponDiscount.toFixed(2)}</span>`);
      updateCheckoutTotal();
    }).fail(function(x) {
      appliedCoupon = null;
      couponDiscount = 0;
      $('#coupon-result').html(`<span class="text-danger"><i class="bi bi-x-circle me-1"></i>${x.responseJSON?.error || 'Invalid coupon'}</span>`);
      updateCheckoutTotal();
    });
  });

  $('#checkout-btn').on('click', function() {
    if (getCart().length === 0) return;
    loadCheckoutOptions();
    bootstrap.Modal.getOrCreateInstance(document.getElementById('checkoutModal')).show();
  });

  $('#place-order-btn').on('click', function() {
    const email = $('#co-email').val().trim();
    if (!email) {
      alert('Email is required');
      return;
    }
    
    const cart = getCart();
    const items = cart.map(x => ({ 
      product_id: x.product_id, 
      variant_id: x.variant_id, 
      quantity: x.qty 
    }));
    
    const payload = {
      items,
      guest_email: email,
      guest_name: $('#co-name').val() || null,
      guest_phone: $('#co-phone').val() || null,
      shipping_address: {
        address: $('#co-address').val() || '',
        city: $('#co-city').val() || '',
        postal_code: $('#co-postal').val() || ''
      },
      shipping_method_id: selectedShipping?.id || null,
      payment_method: $('input[name="payment"]:checked').val() || 'cash',
      coupon_code: appliedCoupon?.code || null,
      notes: $('#co-notes').val() || null
    };
    
    $(this).prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span>Processing...');
    
    $.ajax({
      url: '/api/orders',
      method: 'POST',
      contentType: 'application/json',
      data: JSON.stringify(payload)
    }).then(function(res) {
      setCart([]);
      appliedCoupon = null;
      couponDiscount = 0;
      
      bootstrap.Modal.getInstance(document.getElementById('checkoutModal')).hide();
      $('#success-order-num').text(res.order_number);
      $('#print-invoice').attr('href', '/invoice.html?order=' + encodeURIComponent(res.order_number));
      bootstrap.Modal.getOrCreateInstance(document.getElementById('successModal')).show();
      
      // Reset form
      $('#co-email, #co-name, #co-phone, #co-address, #co-city, #co-postal, #co-coupon, #co-notes').val('');
      $('#coupon-result').html('');
    }).fail(function(x) {
      alert(x.responseJSON?.error || 'Order failed. Please try again.');
    }).always(function() {
      $('#place-order-btn').prop('disabled', false).html('<i class="bi bi-check-lg me-1"></i>Place Order');
    });
  });

  $('#print-invoice').on('click', function(e) {
    e.preventDefault();
    window.open(this.href, '_blank', 'width=500,height=700');
  });

  // ═══════════════════════════════════════════════════════════════
  // INIT
  // ═══════════════════════════════════════════════════════════════
  
  $.get('/api/settings').then(function(s) {
    settings = s;
    if (s.site_name) {
      $('#nav-site-name').text(s.site_name);
      document.title = s.site_name + ' | Multi-Purpose E-Commerce';
    }
  });
  
  loadLocale(lang).then(applyLang);
  loadFilters();
  loadProducts(1);
  renderCart();
})();
