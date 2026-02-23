# Ierahkwa Futurehead Shop

## Multi-Purpose E-Commerce Solution v2.0

Complete E-Commerce system with a **powerful admin panel** for managing products, categories, suppliers, orders and much more.

**Ierahkwa Futurehead Mamey Node** • **IGT-MARKET** • **Ierahkwa Sovereign Blockchain**

Sovereign Government of Ierahkwa Ne Kanienke • Office of the Prime Minister

---

## Highlighted Features

### Design & User Experience
- Clean and elegant design
- Fully responsive design (Bootstrap 5)
- Modern dark theme with gold accents
- Multi-language support (EN, ES, FR, MOH - Kanien'kéha)

### Product Management
- Unlimited product categories and sub-categories
- Easy product entry with rich descriptions
- Generate product barcode automatically
- Product variants (color, size, storage, etc.)
- Product attributes management
- Product discounts and compare prices
- Product images gallery
- Featured and new arrival flags
- SKU and barcode management
- Brands management
- Units management (piece, kg, liter, etc.)

### Inventory Management
- Real-time stock tracking
- Low stock alerts and threshold settings
- Stock adjustment with logs
- Inventory reports
- Multiple warehouses support

### POS & Barcode
- Scanning barcode from web browser
- Quick product lookup by barcode/SKU
- POS-ready interface

### Customer Management
- Customer database with full details
- Customer groups (Regular, VIP, Wholesale)
- Order history per customer
- Customer loyalty points (structure ready)

### Order Management
- Easily manage orders with full lifecycle
- Order statuses: Pending, Confirmed, Processing, Shipped, Delivered, Cancelled, Refunded
- Payment status tracking
- Order history/timeline
- Print order invoice
- Guest checkout option

### Suppliers Management
- Supplier database
- Contact information
- Payment terms tracking

### Shipping & Fulfillment
- Multiple shipping methods
- Free shipping threshold
- Store pickup option
- Shipping cost calculation

### Payments
- Multiple payment methods
- Cash, Credit Card, Bank Transfer
- **IGT Token payments** (Ierahkwa Sovereign Blockchain)
- Payment recording and tracking

### Coupons & Promotions
- Coupon codes (percent, fixed, free shipping)
- Minimum order requirements
- Usage limits
- Expiration dates

### Configuration
- Configure branches/stores
- Configure suppliers
- Configure item types
- Configure tax rates
- Configure home page sliding images
- Configure site logo
- Set social media links
- Set app links (iOS, Android)
- Set site currency symbol
- Dynamically setup meta tags and description
- Configure Google Analytics
- Configure VAT

### Reports & Analytics
- Detailed sales reports
- Daily sales chart
- Monthly sales chart
- Top selling products
- Inventory reports
- Revenue analytics

### Security & Administration
- Admin users management
- Role-based permissions
- Activity logs
- Inventory change logs

---

## Tech Stack

| Component | Technology |
|-----------|------------|
| Runtime | Node.js >= 14 |
| Framework | Fastify 4.x |
| Database | JSON (file-based, zero config) |
| Frontend | Bootstrap 5, jQuery |
| Icons | Bootstrap Icons |
| Auth | bcryptjs, Base64 tokens |

---

## Requirements

- **Node.js** >= 14.0.0
- No external database required (uses JSON file storage)
- Works on any platform (Windows, macOS, Linux)

---

## Installation

```bash
cd ierahkwa-shop
npm install
npm start
```

That's it! The system will auto-initialize with sample data.

---

## Access

| Service | URL |
|---------|-----|
| **Shop** | http://localhost:3100 |
| **Admin Panel** | http://localhost:3100/admin |
| **API** | http://localhost:3100/api |
| **Health Check** | http://localhost:3100/api/health |

### Default Admin Login
- **Email:** `admin@ierahkwa.gov`
- **Password:** `admin123`

---

## Project Structure

```
ierahkwa-shop/
├── config/             # Configuration
├── data/               # JSON database (auto-created)
├── public/             # Frontend static files
│   ├── admin/          # Admin panel
│   ├── js/             # JavaScript
│   ├── locales/        # Language files (en, es, fr, moh)
│   └── index.html      # Shop frontend
├── src/
│   ├── db.js           # Database module
│   └── routes/
│       ├── shop.js     # Public API routes
│       └── admin.js    # Admin API routes
├── server.js           # Main entry point
├── package.json
└── README.md
```

---

## API Reference

### Public Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | List products with filters |
| GET | `/api/products/slug/:slug` | Get product by slug |
| GET | `/api/products/barcode/:code` | Get product by barcode |
| GET | `/api/products/:id/variants` | Get product variants |
| GET | `/api/categories` | List categories (tree) |
| GET | `/api/brands` | List brands |
| GET | `/api/filters` | Get available filters |
| GET | `/api/slider` | Get slider images |
| GET | `/api/settings` | Get public settings |
| GET | `/api/shipping-methods` | List shipping methods |
| GET | `/api/payment-methods` | List payment methods |
| POST | `/api/coupons/validate` | Validate coupon code |
| POST | `/api/orders` | Create order |
| GET | `/api/orders/:orderNumber` | Get order details |
| GET | `/api/orders/:orderNumber/track` | Track order |
| GET | `/api/pages/:slug` | Get page content |
| GET | `/api/products/:id/reviews` | Get product reviews |
| POST | `/api/products/:id/reviews` | Submit review |

### Admin Endpoints (requires auth)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/admin/login` | Admin login |
| GET | `/api/admin/dashboard` | Dashboard stats |
| GET/POST/PUT/DELETE | `/api/admin/products` | Manage products |
| GET/POST/PUT/DELETE | `/api/admin/products/:id/variants` | Manage variants |
| GET/POST/PUT/DELETE | `/api/admin/categories` | Manage categories |
| GET/POST/PUT/DELETE | `/api/admin/suppliers` | Manage suppliers |
| GET | `/api/admin/orders` | List orders |
| GET | `/api/admin/orders/:id` | Get order |
| PATCH | `/api/admin/orders/:id/status` | Update order status |
| PATCH | `/api/admin/orders/:id/payment` | Record payment |
| GET/POST/PUT | `/api/admin/customers` | Manage customers |
| GET | `/api/admin/reports/sales` | Sales report |
| GET | `/api/admin/reports/products` | Product performance |
| GET | `/api/admin/reports/inventory` | Inventory report |
| GET/POST | `/api/admin/settings` | Site settings |
| GET | `/api/admin/users` | List admin users |
| POST | `/api/admin/users` | Create admin user |
| GET | `/api/admin/roles` | List roles |
| GET | `/api/admin/activity` | Activity logs |
| GET | `/api/admin/inventory-logs` | Inventory logs |

---

## Ierahkwa Platform Integration

| Property | Value |
|----------|-------|
| **Node** | Ierahkwa Futurehead Mamey Node |
| **Blockchain** | Ierahkwa Sovereign Blockchain (ISB) |
| **Token** | IGT-MARKET |
| **Bank** | Ierahkwa Futurehead BDET Bank |
| **Domain** | shop.ierahkwa.gov |

Registered in:
- `platform-services.json`
- `NOMENCLATURA-OFICIAL.md`
- `cryptohost-infrastructure.json`
- `REPORTE-EJECUTIVO-COMPLETO-2026.md`
- `tokens/66-IGT-MARKET.json`

---

## License

Proprietary - Sovereign Government of Ierahkwa Ne Kanienke

---

*Sovereign Government of Ierahkwa Ne Kanienke • Office of the Prime Minister*
