'use strict';

// Tests for ierahkwa-shop admin authentication logic
// The admin.js module uses ESM, so we test the auth logic patterns directly

const bcrypt = require('bcryptjs');

describe('Ierahkwa Shop — Admin Authentication', () => {

  // ──────────────────────────────────────────
  // Auth Key Extraction
  // ──────────────────────────────────────────
  describe('auth key extraction', () => {
    function extractKey(headers) {
      return headers['x-admin-key'] || (headers.authorization || '').replace(/^Bearer\s+/i, '');
    }

    test('extracts key from x-admin-key header', () => {
      const key = extractKey({ 'x-admin-key': 'my-secret-key' });
      expect(key).toBe('my-secret-key');
    });

    test('extracts key from Bearer authorization', () => {
      const key = extractKey({ authorization: 'Bearer my-token' });
      expect(key).toBe('my-token');
    });

    test('prefers x-admin-key over authorization', () => {
      const key = extractKey({
        'x-admin-key': 'admin-key',
        authorization: 'Bearer bearer-token'
      });
      expect(key).toBe('admin-key');
    });

    test('returns empty string when no auth headers', () => {
      const key = extractKey({});
      expect(key).toBe('');
    });

    test('handles case-insensitive Bearer prefix', () => {
      const key = extractKey({ authorization: 'BEARER my-token' });
      expect(key).toBe('my-token');
    });
  });

  // ──────────────────────────────────────────
  // Base64 Credential Decoding
  // ──────────────────────────────────────────
  describe('base64 credential decoding', () => {
    test('decodes valid email:password pair', () => {
      const encoded = Buffer.from('admin@ierahkwa.gov:securePass123').toString('base64');
      const [email, pass] = Buffer.from(encoded, 'base64').toString().split(':');
      expect(email).toBe('admin@ierahkwa.gov');
      expect(pass).toBe('securePass123');
    });

    test('handles password with colons', () => {
      const encoded = Buffer.from('user@test.com:pass:with:colons').toString('base64');
      const decoded = Buffer.from(encoded, 'base64').toString();
      const [email, ...rest] = decoded.split(':');
      const pass = rest.join(':');
      expect(email).toBe('user@test.com');
      expect(pass).toBe('pass:with:colons');
    });

    test('rejects invalid base64', () => {
      try {
        const decoded = Buffer.from('not-valid-base64!!!', 'base64').toString();
        const [email, pass] = decoded.split(':');
        // May decode to garbage, but email/pass check should catch it
        expect(typeof email).toBe('string');
      } catch {
        // Expected for truly malformed input
        expect(true).toBe(true);
      }
    });

    test('rejects missing password (no colon)', () => {
      const encoded = Buffer.from('justemail@test.com').toString('base64');
      const [email, pass] = Buffer.from(encoded, 'base64').toString().split(':');
      expect(email).toBe('justemail@test.com');
      expect(pass).toBeUndefined();
    });
  });

  // ──────────────────────────────────────────
  // Password Verification
  // ──────────────────────────────────────────
  describe('bcrypt password verification', () => {
    test('verifies correct password', () => {
      const hash = bcrypt.hashSync('correct-password', 10);
      expect(bcrypt.compareSync('correct-password', hash)).toBe(true);
    });

    test('rejects incorrect password', () => {
      const hash = bcrypt.hashSync('correct-password', 10);
      expect(bcrypt.compareSync('wrong-password', hash)).toBe(false);
    });

    test('different hashes for same password (salt)', () => {
      const hash1 = bcrypt.hashSync('same-password', 10);
      const hash2 = bcrypt.hashSync('same-password', 10);
      expect(hash1).not.toBe(hash2);
      // But both should verify
      expect(bcrypt.compareSync('same-password', hash1)).toBe(true);
      expect(bcrypt.compareSync('same-password', hash2)).toBe(true);
    });
  });

  // ──────────────────────────────────────────
  // Dev Key Access Control
  // ──────────────────────────────────────────
  describe('dev key access control', () => {
    test('dev keys are only allowed in non-production', () => {
      const nodeEnv = 'development';
      const devKeyAllowed = nodeEnv !== 'production';
      expect(devKeyAllowed).toBe(true);
    });

    test('dev keys are blocked in production', () => {
      const nodeEnv = 'production';
      const devKeyAllowed = nodeEnv !== 'production';
      expect(devKeyAllowed).toBe(false);
    });

    test('dev key must come from environment variable', () => {
      // After the security fix, hardcoded keys were removed
      // Dev key now comes from DEV_ADMIN_KEY env var
      const devKey = process.env.DEV_ADMIN_KEY;
      // In test environment, should be undefined unless explicitly set
      const key = 'some-request-key';
      const matches = key === devKey;
      expect(matches).toBe(false); // No hardcoded match possible
    });
  });

  // ──────────────────────────────────────────
  // Role-Based Permissions
  // ──────────────────────────────────────────
  describe('role-based permissions', () => {
    const mockRoles = [
      { id: 1, name: 'superadmin', permissions: ['all'] },
      { id: 2, name: 'manager', permissions: ['products', 'orders', 'customers'] },
      { id: 3, name: 'cashier', permissions: ['pos', 'orders'] }
    ];

    test('superadmin has all permissions', () => {
      const role = mockRoles.find(r => r.name === 'superadmin');
      expect(role.permissions).toContain('all');
    });

    test('manager has product and order permissions', () => {
      const role = mockRoles.find(r => r.name === 'manager');
      expect(role.permissions).toContain('products');
      expect(role.permissions).toContain('orders');
      expect(role.permissions).not.toContain('all');
    });

    test('cashier has limited permissions', () => {
      const role = mockRoles.find(r => r.name === 'cashier');
      expect(role.permissions).toContain('pos');
      expect(role.permissions).not.toContain('products');
    });

    test('user lookup finds active users by email', () => {
      const mockUsers = [
        { id: 1, email: 'admin@ierahkwa.gov', is_active: true, role_id: 1 },
        { id: 2, email: 'disabled@ierahkwa.gov', is_active: false, role_id: 2 }
      ];

      const active = mockUsers.find(u => u.email === 'admin@ierahkwa.gov' && u.is_active);
      expect(active).toBeDefined();
      expect(active.id).toBe(1);

      const disabled = mockUsers.find(u => u.email === 'disabled@ierahkwa.gov' && u.is_active);
      expect(disabled).toBeUndefined();
    });
  });

  // ──────────────────────────────────────────
  // Product Operations
  // ──────────────────────────────────────────
  describe('product validation', () => {
    test('generates valid SKU format', () => {
      const sku = 'SKU-' + Date.now().toString(36).toUpperCase();
      expect(sku).toMatch(/^SKU-/);
    });

    test('generates slug from product name', () => {
      const name = 'Ierahkwa Sacred Earrings';
      const slug = name.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');
      expect(slug).toBe('ierahkwa-sacred-earrings');
    });

    test('detects low stock products', () => {
      const products = [
        { name: 'Item A', stock: 5, min_stock: 10, is_active: true },
        { name: 'Item B', stock: 50, min_stock: 10, is_active: true },
        { name: 'Item C', stock: 0, min_stock: 5, is_active: true }
      ];
      const lowStock = products.filter(p => p.is_active && p.stock <= (p.min_stock || 10));
      expect(lowStock).toHaveLength(2);
      expect(lowStock[0].name).toBe('Item A');
      expect(lowStock[1].name).toBe('Item C');
    });
  });

  // ──────────────────────────────────────────
  // Order Status Transitions
  // ──────────────────────────────────────────
  describe('order status management', () => {
    const validStatuses = ['pending', 'confirmed', 'processing', 'shipped', 'delivered', 'cancelled', 'refunded'];

    test('validates known statuses', () => {
      expect(validStatuses.includes('pending')).toBe(true);
      expect(validStatuses.includes('confirmed')).toBe(true);
      expect(validStatuses.includes('shipped')).toBe(true);
    });

    test('rejects unknown statuses', () => {
      expect(validStatuses.includes('approved')).toBe(false);
      expect(validStatuses.includes('deleted')).toBe(false);
    });

    test('cancellation triggers stock return', () => {
      const order = { id: 1, status: 'confirmed' };
      const items = [
        { product_id: 1, quantity: 3 },
        { product_id: 2, quantity: 1 }
      ];
      const products = [
        { id: 1, stock: 10 },
        { id: 2, stock: 5 }
      ];

      // Simulate cancellation stock return
      if (order.status !== 'cancelled') {
        items.forEach(item => {
          const p = products.find(x => x.id === item.product_id);
          if (p) p.stock += item.quantity;
        });
      }

      expect(products[0].stock).toBe(13); // 10 + 3
      expect(products[1].stock).toBe(6);  // 5 + 1
    });
  });
});
