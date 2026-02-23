/** Rutas públicas - Ierahkwa Futurehead Shop Multi-Purpose E-Commerce */
import db from '../db.js';

export default async function shopRoutes(fastify) {
  // ═══════════════════════════════════════════════════════════════
  // PRODUCTOS
  // ═══════════════════════════════════════════════════════════════
  
  // Lista de productos con filtros avanzados
  fastify.get('/api/products', async (req) => {
    const { q, category, brand, color, size, min_price, max_price, sort, featured, new_arrivals, page = 1, limit = 24 } = req.query;
    const data = db.get();
    let list = (data.products || []).filter(p => p.is_active);
    
    // Búsqueda
    if (q) {
      const ql = q.toLowerCase();
      list = list.filter(p => 
        (p.name || '').toLowerCase().includes(ql) || 
        (p.sku || '').toLowerCase().includes(ql) || 
        (p.description || '').toLowerCase().includes(ql) ||
        p.barcode === q
      );
    }
    
    // Filtros
    if (category) {
      const cid = parseInt(category, 10);
      const cats = data.categories || [];
      const getChildIds = (pid) => {
        const children = cats.filter(c => c.parent_id === pid).map(c => c.id);
        return [pid, ...children.flatMap(getChildIds)];
      };
      const catIds = getChildIds(cid);
      list = list.filter(p => catIds.includes(p.category_id));
    }
    if (brand) list = list.filter(p => p.brand_id === parseInt(brand, 10));
    if (color) list = list.filter(p => p.attributes?.color === color);
    if (size) list = list.filter(p => p.attributes?.size === size);
    if (min_price) list = list.filter(p => db.calcPrice(p) >= parseFloat(min_price));
    if (max_price) list = list.filter(p => db.calcPrice(p) <= parseFloat(max_price));
    if (featured === '1') list = list.filter(p => p.is_featured);
    if (new_arrivals === '1') list = list.filter(p => p.is_new);

    // Ordenar
    if (sort === 'price_asc') list.sort((a, b) => db.calcPrice(a) - db.calcPrice(b));
    else if (sort === 'price_desc') list.sort((a, b) => db.calcPrice(b) - db.calcPrice(a));
    else if (sort === 'name_asc') list.sort((a, b) => (a.name || '').localeCompare(b.name || ''));
    else if (sort === 'newest') list.sort((a, b) => (b.created_at || '').localeCompare(a.created_at || ''));
    else list.sort((a, b) => (b.is_featured ? 1 : 0) - (a.is_featured ? 1 : 0));

    // Enriquecer datos
    list = list.map(p => {
      const cat = (data.categories || []).find(c => c.id === p.category_id);
      const brand = (data.brands || []).find(b => b.id === p.brand_id);
      return { 
        ...p, 
        category_name: cat?.name || '', 
        brand_name: brand?.name || '',
        final_price: db.calcPrice(p),
        has_variants: p.type === 'variable'
      };
    });

    const total = list.length;
    const off = (Math.max(1, parseInt(page, 10)) - 1) * Math.min(100, parseInt(limit, 10) || 24);
    const paged = list.slice(off, off + (parseInt(limit, 10) || 24));
    
    return { 
      data: paged, 
      total, 
      page: parseInt(page, 10), 
      limit: parseInt(limit, 10) || 24,
      pages: Math.ceil(total / (parseInt(limit, 10) || 24))
    };
  });

  // Producto por slug
  fastify.get('/api/products/slug/:slug', async (req, reply) => {
    const data = db.get();
    const p = (data.products || []).find(x => x.slug === req.params.slug && x.is_active);
    if (!p) return reply.code(404).send({ error: 'Product not found' });
    
    const cat = (data.categories || []).find(c => c.id === p.category_id);
    const brand = (data.brands || []).find(b => b.id === p.brand_id);
    const variants = p.type === 'variable' ? (data.product_variants || []).filter(v => v.product_id === p.id && v.is_active) : [];
    const reviews = (data.reviews || []).filter(r => r.product_id === p.id && r.is_approved);
    const avgRating = reviews.length ? reviews.reduce((s, r) => s + r.rating, 0) / reviews.length : 0;
    
    // Productos relacionados
    const related = (data.products || [])
      .filter(x => x.id !== p.id && x.category_id === p.category_id && x.is_active)
      .slice(0, 4)
      .map(x => ({ id: x.id, name: x.name, slug: x.slug, price: x.price, final_price: db.calcPrice(x), images: x.images }));
    
    return { 
      ...p, 
      category_name: cat?.name || '', 
      brand_name: brand?.name || '',
      final_price: db.calcPrice(p),
      variants,
      reviews_count: reviews.length,
      avg_rating: Math.round(avgRating * 10) / 10,
      related
    };
  });

  // Producto por barcode (POS scanning)
  fastify.get('/api/products/barcode/:code', async (req, reply) => {
    const data = db.get();
    const code = req.params.code;
    
    // Buscar en productos
    let p = (data.products || []).find(x => x.barcode === code && x.is_active);
    let variant = null;
    
    // Buscar en variantes
    if (!p) {
      variant = (data.product_variants || []).find(v => v.barcode === code && v.is_active);
      if (variant) p = (data.products || []).find(x => x.id === variant.product_id && x.is_active);
    }
    
    if (!p) return reply.code(404).send({ error: 'Product not found' });
    
    return { ...p, variant, final_price: variant ? variant.price : db.calcPrice(p) };
  });

  // Variantes de producto
  fastify.get('/api/products/:id/variants', async (req) => {
    const variants = (db.get().product_variants || []).filter(v => v.product_id === parseInt(req.params.id, 10) && v.is_active);
    return { data: variants };
  });

  // ═══════════════════════════════════════════════════════════════
  // CATEGORÍAS
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/categories', async () => {
    const cats = (db.get().categories || []).filter(c => c.is_active);
    
    // Construir árbol
    const buildTree = (parentId = null) => {
      return cats
        .filter(c => c.parent_id === parentId)
        .sort((a, b) => a.sort_order - b.sort_order)
        .map(c => ({
          ...c,
          children: buildTree(c.id)
        }));
    };
    
    return { data: buildTree(null), flat: cats };
  });

  fastify.get('/api/categories/:slug', async (req, reply) => {
    const cat = (db.get().categories || []).find(c => c.slug === req.params.slug && c.is_active);
    if (!cat) return reply.code(404).send({ error: 'Category not found' });
    return cat;
  });

  // ═══════════════════════════════════════════════════════════════
  // MARCAS
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/brands', async () => {
    return { data: (db.get().brands || []).filter(b => b.is_active) };
  });

  // ═══════════════════════════════════════════════════════════════
  // FILTROS DISPONIBLES
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/filters', async () => {
    const data = db.get();
    const prods = (data.products || []).filter(p => p.is_active);
    const variants = (data.product_variants || []).filter(v => v.is_active);
    
    const colors = new Set();
    const sizes = new Set();
    
    prods.forEach(p => {
      if (p.attributes?.color) colors.add(p.attributes.color);
      if (p.attributes?.size) sizes.add(p.attributes.size);
    });
    variants.forEach(v => {
      if (v.attributes?.color) colors.add(v.attributes.color);
      if (v.attributes?.size) sizes.add(v.attributes.size);
    });
    
    const prices = prods.map(p => db.calcPrice(p)).filter(p => p > 0);
    
    return { 
      colors: [...colors].sort(), 
      sizes: [...sizes].sort(),
      price_range: {
        min: prices.length ? Math.min(...prices) : 0,
        max: prices.length ? Math.max(...prices) : 0
      },
      brands: (data.brands || []).filter(b => b.is_active).map(b => ({ id: b.id, name: b.name }))
    };
  });

  // ═══════════════════════════════════════════════════════════════
  // SLIDER Y BANNERS
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/slider', async () => {
    return { data: (db.get().slider_images || []).filter(s => s.is_active).sort((a, b) => a.sort_order - b.sort_order) };
  });

  fastify.get('/api/banners', async () => {
    return { data: (db.get().banners || []).filter(b => b.is_active) };
  });

  // ═══════════════════════════════════════════════════════════════
  // CONFIGURACIÓN PÚBLICA
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/settings', async () => {
    const s = db.get().settings || {};
    return {
      site_name: s.site_name,
      site_tagline: s.site_tagline,
      site_logo: s.site_logo,
      currency_symbol: s.currency_symbol || '$',
      currency_code: s.currency_code || 'USD',
      vat_percent: s.vat_percent,
      vat_label: s.vat_label,
      guest_checkout: s.guest_checkout,
      enable_reviews: s.enable_reviews,
      enable_wishlist: s.enable_wishlist,
      shipping_enabled: s.shipping_enabled,
      free_shipping_min: s.free_shipping_min,
      social: {
        facebook: s.social_facebook,
        twitter: s.social_twitter,
        instagram: s.social_instagram,
        youtube: s.social_youtube
      },
      apps: {
        ios: s.app_ios,
        android: s.app_android
      },
      contact: {
        email: s.contact_email,
        phone: s.contact_phone,
        address: s.contact_address
      },
      ierahkwa: {
        node: 'Ierahkwa Futurehead Mamey Node',
        blockchain: 'Ierahkwa Sovereign Blockchain',
        bank: 'Ierahkwa Futurehead BDET Bank',
        government: 'Sovereign Government of Ierahkwa Ne Kanienke',
        token: 'IGT-MARKET'
      }
    };
  });

  // ═══════════════════════════════════════════════════════════════
  // MÉTODOS DE ENVÍO Y PAGO
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/shipping-methods', async () => {
    return { data: (db.get().shipping_methods || []).filter(s => s.is_active) };
  });

  fastify.get('/api/payment-methods', async () => {
    return { data: (db.get().payment_methods || []).filter(p => p.is_active) };
  });

  // ═══════════════════════════════════════════════════════════════
  // CUPONES
  // ═══════════════════════════════════════════════════════════════
  
  fastify.post('/api/coupons/validate', async (req, reply) => {
    const { code, subtotal } = req.body || {};
    if (!code) return reply.code(400).send({ error: 'Coupon code required' });
    
    const coupon = (db.get().coupons || []).find(c => 
      c.code.toUpperCase() === code.toUpperCase() && 
      c.is_active &&
      (!c.expires_at || c.expires_at >= db.today()) &&
      (!c.starts_at || c.starts_at <= db.today()) &&
      (!c.usage_limit || c.used_count < c.usage_limit)
    );
    
    if (!coupon) return reply.code(400).send({ error: 'Invalid or expired coupon' });
    if (coupon.min_order && subtotal < coupon.min_order) {
      return reply.code(400).send({ error: `Minimum order amount is ${coupon.min_order}` });
    }
    
    let discount = 0;
    if (coupon.type === 'percent') {
      discount = subtotal * (coupon.value / 100);
      if (coupon.max_discount) discount = Math.min(discount, coupon.max_discount);
    } else if (coupon.type === 'fixed') {
      discount = coupon.value;
    }
    
    return { 
      valid: true, 
      coupon: { code: coupon.code, name: coupon.name, type: coupon.type, value: coupon.value },
      discount
    };
  });

  // ═══════════════════════════════════════════════════════════════
  // ÓRDENES
  // ═══════════════════════════════════════════════════════════════
  
  fastify.post('/api/orders', async (req, reply) => {
    const { 
      customer_id, guest_email, guest_name, guest_phone,
      items, shipping_address, billing_address,
      shipping_method_id, payment_method,
      coupon_code, notes 
    } = req.body || {};
    
    if (!items || !Array.isArray(items) || items.length === 0) {
      return reply.code(400).send({ error: 'Items required' });
    }

    const data = db.get();
    const settings = data.settings || {};
    const vatPct = parseFloat(settings.vat_percent || '0');
    const orderNum = db.generateOrderNumber();

    let subtotal = 0;
    const orderItems = [];

    for (const it of items) {
      const prod = (data.products || []).find(p => p.id === it.product_id && p.is_active);
      if (!prod) continue;
      
      let variant = null;
      let price = db.calcPrice(prod);
      let stock = prod.stock;
      let sku = prod.sku;
      let barcode = prod.barcode;
      
      // Si tiene variante
      if (it.variant_id) {
        variant = (data.product_variants || []).find(v => v.id === it.variant_id && v.product_id === prod.id && v.is_active);
        if (variant) {
          price = variant.price;
          stock = variant.stock;
          sku = variant.sku;
          barcode = variant.barcode;
        }
      }
      
      const qty = Math.max(1, parseInt(it.quantity, 10) || 1);
      if (stock < qty) continue;
      
      const itemTotal = price * qty;
      subtotal += itemTotal;
      
      orderItems.push({
        id: Date.now() + Math.random(),
        product_id: prod.id,
        variant_id: variant?.id || null,
        product_name: prod.name,
        variant_name: variant ? Object.values(variant.attributes).join(' / ') : null,
        sku,
        barcode,
        quantity: qty,
        unit_price: price,
        total: itemTotal,
        attributes: variant?.attributes || prod.attributes || {}
      });
      
      // Actualizar stock
      if (variant) {
        variant.stock -= qty;
      } else {
        prod.stock -= qty;
      }
      
      // Log de inventario
      db.logInventory(prod.id, variant?.id, 'out', qty, orderNum, 'Sale');
    }

    if (orderItems.length === 0) return reply.code(400).send({ error: 'No valid items' });

    // Calcular descuento de cupón
    let discountAmount = 0;
    let appliedCoupon = null;
    if (coupon_code) {
      const coupon = (data.coupons || []).find(c => 
        c.code.toUpperCase() === coupon_code.toUpperCase() && c.is_active
      );
      if (coupon) {
        if (coupon.type === 'percent') {
          discountAmount = subtotal * (coupon.value / 100);
          if (coupon.max_discount) discountAmount = Math.min(discountAmount, coupon.max_discount);
        } else if (coupon.type === 'fixed') {
          discountAmount = coupon.value;
        }
        coupon.used_count = (coupon.used_count || 0) + 1;
        appliedCoupon = coupon.code;
      }
    }

    // Calcular envío
    let shippingAmount = 0;
    const shippingMethod = (data.shipping_methods || []).find(s => s.id === shipping_method_id);
    if (shippingMethod) {
      shippingAmount = shippingMethod.price;
      // Free shipping
      const freeMin = parseFloat(settings.free_shipping_min || '0');
      if (freeMin > 0 && subtotal >= freeMin) shippingAmount = 0;
      if (appliedCoupon) {
        const coup = (data.coupons || []).find(c => c.code.toUpperCase() === appliedCoupon.toUpperCase());
        if (coup?.type === 'free_shipping') shippingAmount = 0;
      }
    }

    const vatAmount = (subtotal - discountAmount) * (vatPct / 100);
    const total = subtotal - discountAmount + vatAmount + shippingAmount;
    const branchId = (data.branches || []).find(b => b.is_default)?.id || 1;

    const oid = db.nextId('orders');
    const order = {
      id: oid,
      order_number: orderNum,
      customer_id: customer_id || null,
      branch_id: branchId,
      guest_email: guest_email || null,
      guest_name: guest_name || null,
      guest_phone: guest_phone || null,
      shipping_address: shipping_address || null,
      billing_address: billing_address || null,
      shipping_method_id: shipping_method_id || null,
      shipping_method_name: shippingMethod?.name || null,
      payment_method: payment_method || 'cash',
      coupon_code: appliedCoupon,
      status: 'pending',
      payment_status: 'unpaid',
      subtotal,
      discount_amount: discountAmount,
      vat_amount: vatAmount,
      shipping_amount: shippingAmount,
      total,
      currency: settings.currency_code || 'USD',
      notes: notes || null,
      created_at: db.now(),
      updated_at: db.now()
    };

    data.orders = data.orders || [];
    data.orders.push(order);
    
    data.order_items = data.order_items || [];
    orderItems.forEach(oi => { oi.order_id = oid; data.order_items.push(oi); });

    // Historial de orden
    data.order_history = data.order_history || [];
    data.order_history.push({
      id: Date.now(),
      order_id: oid,
      status: 'pending',
      notes: 'Order placed',
      created_at: db.now()
    });

    db.save();
    db.logActivity(null, 'order_created', 'order', oid, { order_number: orderNum, total });

    return { 
      order_id: oid, 
      order_number: orderNum, 
      subtotal,
      discount_amount: discountAmount,
      vat_amount: vatAmount,
      shipping_amount: shippingAmount,
      total, 
      status: 'pending',
      items_count: orderItems.length
    };
  });

  // Detalle de orden
  fastify.get('/api/orders/:orderNumber', async (req, reply) => {
    const data = db.get();
    const o = (data.orders || []).find(x => x.order_number === req.params.orderNumber);
    if (!o) return reply.code(404).send({ error: 'Order not found' });
    
    const branch = (data.branches || []).find(b => b.id === o.branch_id);
    const items = (data.order_items || []).filter(i => i.order_id === o.id);
    const history = (data.order_history || []).filter(h => h.order_id === o.id).sort((a, b) => (a.created_at || '').localeCompare(b.created_at || ''));
    
    return { ...o, branch_name: branch?.name || '', items, history };
  });

  // Tracking de orden
  fastify.get('/api/orders/:orderNumber/track', async (req, reply) => {
    const data = db.get();
    const o = (data.orders || []).find(x => x.order_number === req.params.orderNumber);
    if (!o) return reply.code(404).send({ error: 'Order not found' });
    
    const history = (data.order_history || []).filter(h => h.order_id === o.id).sort((a, b) => (a.created_at || '').localeCompare(b.created_at || ''));
    
    return { 
      order_number: o.order_number,
      status: o.status,
      payment_status: o.payment_status,
      created_at: o.created_at,
      history
    };
  });

  // ═══════════════════════════════════════════════════════════════
  // PÁGINAS
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/pages/:slug', async (req, reply) => {
    const page = (db.get().pages || []).find(p => p.slug === req.params.slug && p.is_active);
    if (!page) return reply.code(404).send({ error: 'Page not found' });
    return page;
  });

  // ═══════════════════════════════════════════════════════════════
  // REVIEWS
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/products/:id/reviews', async (req) => {
    const reviews = (db.get().reviews || [])
      .filter(r => r.product_id === parseInt(req.params.id, 10) && r.is_approved)
      .sort((a, b) => (b.created_at || '').localeCompare(a.created_at || ''));
    return { data: reviews };
  });

  fastify.post('/api/products/:id/reviews', async (req, reply) => {
    const { name, email, rating, title, comment } = req.body || {};
    if (!rating || rating < 1 || rating > 5) return reply.code(400).send({ error: 'Rating 1-5 required' });
    
    const data = db.get();
    const product = (data.products || []).find(p => p.id === parseInt(req.params.id, 10));
    if (!product) return reply.code(404).send({ error: 'Product not found' });
    
    data.reviews = data.reviews || [];
    data.reviews.push({
      id: Date.now(),
      product_id: product.id,
      name: name || 'Anonymous',
      email: email || null,
      rating: parseInt(rating, 10),
      title: title || '',
      comment: comment || '',
      is_approved: false, // Requiere aprobación
      created_at: db.now()
    });
    db.save();
    
    return { ok: true, message: 'Review submitted for approval' };
  });
}
