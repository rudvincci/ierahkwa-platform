# Inventory Manager Pro

Professional Inventory Management System built with DevExpress, C#, and SQLite.

## Features

### Core Functionality
- **User Authentication**: Secure login with password hashing (BCrypt)
- **Multi-User Support**: SQLite WAL mode enables concurrent access from multiple computers
- **Role-Based Access**: Admin, Manager, User, and ReadOnly roles
- **Real-Time Stock Status**: Live tracking of inventory levels

### Product Management
- Product catalog with categories and suppliers
- Barcode support for quick scanning
- Auto-generated product codes
- Product images support
- Stock level alerts (minimum/maximum)

### Stock Operations
- **Stock In**: Purchase entries
- **Stock Out**: Sales entries  
- **Quick Stock Entry**: Barcode scanner support with auto-complete
- **Stock Adjustment**: Inventory corrections
- **Movement History**: Complete audit trail of all stock changes

### Search & Navigation
- **Quick Search (F3)**: Instant product search popup
- **Auto-Complete**: Fast product lookup while typing
- **Advanced Filtering**: Filter by category, supplier, status
- **Grid Search**: Built-in search in all data grids

### Reports & Analytics
- Stock Report with current values
- Low Stock Alert Report
- Movement Summary Report
- Statistics with charts:
  - Stock value by category (Pie chart)
  - Monthly movements (Bar chart)
  - Top products by value

### Export Options
- **Excel** (.xlsx): Full data export with formatting
- **PDF**: Print-ready documents
- **HTML**: Web-viewable reports
- **CSV/TXT**: Plain text exports
- **Print**: Direct printing with preview

### Database Management
- **Backup**: One-click database backup
- **Restore**: Restore from backup files
- **Activity Log**: Complete audit trail
- **Settings**: Configurable application options

## Requirements

- Windows 10/11
- .NET 8.0 or later
- Visual Studio 2022 (for development)
- DevExpress WinForms Components (v23.2+)

## Installation

### For Development

1. Clone or download the project
2. Open `InventoryManager.sln` in Visual Studio 2022
3. Restore NuGet packages (automatic)
4. Build and run

### For Production

1. Build in Release mode
2. Deploy the output folder
3. Ensure .NET 8.0 runtime is installed on target machines

## Configuration

### Database Location
The SQLite database is stored at:
```
%ProgramData%\InventoryManager\inventory.db
```

### Shared Database (Multi-User)
For multi-computer access, place the database on a shared network drive and update the connection string in `DatabaseManager.cs`.

## Default Credentials

- **Username**: admin
- **Password**: admin123

> **Important**: Change the default password after first login!

## Project Structure

```
InventoryManager/
├── Data/               # Database access layer
│   ├── DatabaseManager.cs
│   ├── ProductRepository.cs
│   ├── CategoryRepository.cs
│   ├── SupplierRepository.cs
│   ├── StockMovementRepository.cs
│   └── ...
├── Models/             # Data models
│   ├── Product.cs
│   ├── Category.cs
│   ├── Supplier.cs
│   └── ...
├── Forms/              # WinForms UI
│   ├── MainForm.cs
│   ├── LoginForm.cs
│   ├── ProductsControl.cs
│   └── ...
├── Services/           # Business services
│   └── ExportService.cs
├── Resources/          # Icons and images
└── Help/               # Documentation
```

## Technologies Used

- **DevExpress WinForms**: Professional UI components
  - RibbonControl for main menu
  - GridControl for data display
  - ChartControl for statistics
  - SearchLookUpEdit for autocomplete
  - XtraPrinting for reports
- **SQLite**: Lightweight embedded database
- **Dapper**: Micro ORM for data access
- **BCrypt.Net**: Password hashing
- **Newtonsoft.Json**: JSON serialization
- **ClosedXML**: Excel export support

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| F3 | Quick Search |
| F5 | Refresh |
| Enter | Confirm / Select |
| Escape | Cancel / Close |

## Support

For questions or issues, contact support@inventory.local

## License

Copyright © 2026 Sovereign Akwesasne Government. All Rights Reserved.
