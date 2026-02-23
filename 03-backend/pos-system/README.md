# Smart Business Platform

A complete business management suite with POS, CRM, and Inventory systems built with Node.js.

## Integrated Modules

### 1. POS (Point of Sale)
- Restaurant table management
- Fast checkout with cart
- Multiple payment methods
- Receipt printing
- Daily sales reports

### 2. CRM (Customer Relationship Management)
- Customer management
- Lead tracking
- Support tickets
- Invoicing
- QA reviews

### 3. Inventory Management
- Product catalog
- Multi-warehouse support
- Supplier management
- Stock movements (in/out/transfer)
- Purchase orders
- Stock adjustments
- Low stock alerts
- Inventory valuation reports

## Features

- **Smart Sale Screen** - Easy-to-use interface with item categories, search, and quick cart management
- **Table Management** - Drag-and-drop table layout with real-time status (available, occupied, reserved, cleaning)
- **Order Management** - Full order lifecycle with multiple payment methods
- **Reporting System** - Daily summaries, sales trends, top items, category analysis
- **Multi-language Support** - English, Spanish, Arabic (RTL), and French
- **User Management** - Role-based permissions (Admin, Manager, Cashier)
- **Tax System** - Configurable tax rates per item
- **Receipt Printing** - Print-ready receipts

## Requirements

- Node.js 16+ 
- npm or yarn

## Installation

```bash
cd pos-system
npm install
```

## Running the Application

```bash
npm start
```

The server will start at **http://localhost:3030**

## Default Login Credentials

- **Username:** `a`
- **Password:** `123456`

## Project Structure

```
pos-system/
├── server.js           # Express server entry point
├── src/
│   ├── db.js          # SQLite database setup
│   └── routes/        # API routes
│       ├── auth.js    # Authentication
│       ├── items.js   # Items & categories
│       ├── orders.js  # Order management
│       ├── tables.js  # Table management
│       ├── reports.js # Reporting
│       └── users.js   # User management
├── public/            # Frontend files
│   ├── index.html     # Main application
│   ├── login.html     # Login page
│   ├── css/
│   │   └── style.css  # Styles
│   └── js/
│       ├── api.js     # API client
│       ├── i18n.js    # Internationalization
│       └── app.js     # Main application logic
└── data/              # SQLite database (auto-created)
```

## API Endpoints

### Authentication
- `POST /api/auth/login` - Login
- `POST /api/auth/logout` - Logout
- `GET /api/auth/check` - Check session

### Items
- `GET /api/items` - Get all items
- `GET /api/items/categories` - Get all categories
- `POST /api/items` - Create item
- `PUT /api/items/:id` - Update item
- `DELETE /api/items/:id` - Delete item

### Orders
- `GET /api/orders` - Get orders (with filters)
- `GET /api/orders/:id` - Get order details
- `POST /api/orders` - Create order
- `PUT /api/orders/:id` - Update order
- `POST /api/orders/:id/pay` - Process payment

### Tables
- `GET /api/tables` - Get all tables
- `POST /api/tables` - Create table
- `PUT /api/tables/:id` - Update table
- `PATCH /api/tables/:id/position` - Update table position
- `PATCH /api/tables/:id/status` - Update table status

### Reports
- `GET /api/reports/daily` - Daily summary
- `GET /api/reports/sales` - Sales by date range
- `GET /api/reports/items` - Items report
- `GET /api/reports/categories` - Categories report

### Users (Admin only)
- `GET /api/users` - Get all users
- `POST /api/users` - Create user
- `PUT /api/users/:id` - Update user
- `DELETE /api/users/:id` - Delete user

## Changing Language

Click the globe icon in the sidebar to switch between:
- English
- Español (Spanish)
- العربية (Arabic - RTL)
- Français (French)

## License

MIT License - Sovereign Akwesasne Government
