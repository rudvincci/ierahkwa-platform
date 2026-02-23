# Smart School & Accounting System

A comprehensive multi-tenant school management and accounting system built with .NET 10 using Clean Architecture.

## Features

### School Management
- **Grades** - Create and manage school grades
- **Class Rooms** - Manage class rooms per grade
- **Materials/Subjects** - Define teaching materials
- **Teachers** - Teacher management with material assignments
- **Students** - Student registration and class assignment
- **Parents** - Parent accounts linked to students
- **Schedules** - Weekly class schedules
- **Homeworks** - Homework assignments with content and questions
- **Live Classes** - Zoom integration for online classes

### Accounting Module
- **Fees Management** - Define various fee types
- **Invoices** - Fees and purchase invoices
- **Products & Inventory** - Stock management
- **Suppliers** - Supplier management
- **Accounts Tree** - Chart of accounts
- **Journals** - Journal entries
- **Cost Centers** - Cost center management
- **Reports** - Various financial reports

### Reception Module
- Admission Enquiries
- Visitor Book
- Phone Logs
- Postal Dispatch/Receive
- Complaints Management

### Library Module
- Books Management
- Book Categories
- Library Members
- Borrow/Return Transactions

## Architecture

The system follows Clean Architecture principles:

```
src/
├── Common/                 # Shared kernel
│   ├── Common.Domain      # Base entities, interfaces
│   ├── Common.Application # Common DTOs, interfaces
│   └── Common.Persistence # Base DbContext, repositories
├── UserManagement/        # User & authentication
│   ├── UserManagement.Domain
│   ├── UserManagement.Application
│   └── UserManagement.Persistence
├── OnlineSchool/          # School management
│   ├── OnlineSchool.Domain
│   ├── OnlineSchool.Application
│   └── OnlineSchool.Persistence
├── SmartAccounting/       # Accounting module
│   ├── SmartAccounting.Domain
│   ├── SmartAccounting.Application
│   └── SmartAccounting.Persistence
├── Receptionist/          # Reception module
│   ├── Receptionist.Domain
│   ├── Receptionist.Application
│   └── Receptionist.Persistence
├── Librarian/             # Library module
│   ├── Librarian.Domain
│   ├── Librarian.Application
│   └── Librarian.Persistence
├── Zoom/                  # Zoom integration
│   ├── Zoom.Domain
│   ├── Zoom.Application
│   └── Zoom.Persistence
└── Web/
    └── SmartSchool.Web    # MVC Web Application
```

## User Roles

The system includes 8 default roles:

1. **Admin** - Super administrator (manage all schools)
2. **SchoolAdmin** - School administrator (manage one school)
3. **Accountant** - Financial management
4. **Teacher** - Teaching and grading
5. **Student** - View materials and submit homework
6. **Parent** - View child's progress
7. **Receptionist** - Reception desk management
8. **Librarian** - Library management

## Technology Stack

- **.NET 10** - Framework
- **ASP.NET Core MVC** - Web Framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **JWT** - Authentication
- **Bootstrap 5** - UI Framework
- **NLog** - Logging

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run the application (database will be created automatically)

```bash
cd SmartSchool/src/Web/SmartSchool.Web
dotnet run
```

### Default Credentials

- **Username:** admin
- **Password:** P@ssw0rd

### Database Migrations (Optional)

To use migrations instead of EnsureCreated:

```bash
# Common
dotnet ef migrations add Initial -c CommonDbContext -p src/Common/Common.Persistence -s src/Web/SmartSchool.Web
dotnet ef database update -c CommonDbContext -s src/Web/SmartSchool.Web

# UserManagement
dotnet ef migrations add Initial -c UserManagementDbContext -p src/UserManagement/UserManagement.Persistence -s src/Web/SmartSchool.Web
dotnet ef database update -c UserManagementDbContext -s src/Web/SmartSchool.Web

# OnlineSchool
dotnet ef migrations add Initial -c OnlineSchoolDbContext -p src/OnlineSchool/OnlineSchool.Persistence -s src/Web/SmartSchool.Web
dotnet ef database update -c OnlineSchoolDbContext -s src/Web/SmartSchool.Web

# SmartAccounting
dotnet ef migrations add Initial -c SmartAccountingDbContext -p src/SmartAccounting/SmartAccounting.Persistence -s src/Web/SmartSchool.Web
dotnet ef database update -c SmartAccountingDbContext -s src/Web/SmartSchool.Web

# Receptionist
dotnet ef migrations add Initial -c ReceptionistDbContext -p src/Receptionist/Receptionist.Persistence -s src/Web/SmartSchool.Web
dotnet ef database update -c ReceptionistDbContext -s src/Web/SmartSchool.Web

# Librarian
dotnet ef migrations add Initial -c LibrarianDbContext -p src/Librarian/Librarian.Persistence -s src/Web/SmartSchool.Web
dotnet ef database update -c LibrarianDbContext -s src/Web/SmartSchool.Web

# Zoom
dotnet ef migrations add Initial -c ZoomDbContext -p src/Zoom/Zoom.Persistence -s src/Web/SmartSchool.Web
dotnet ef database update -c ZoomDbContext -s src/Web/SmartSchool.Web
```

## Quick Start Guide

1. Login with admin credentials
2. Create a School (Tenant)
3. Create a School Admin for the school
4. Logout and login as School Admin
5. Create Grades, Class Rooms, Materials
6. Create Teachers and assign materials
7. Create Students and assign to classes
8. Create Parents and link to students
9. Teachers can create homeworks and live classes
10. Students can view content and submit answers
11. Accountants can manage fees and invoices

## Multi-Tenancy

The system supports multiple schools (tenants). Each school has its own:
- Grades and Class Rooms
- Teachers, Students, Parents
- Fees and Invoices
- Library items
- Reception records

The Super Admin can create and manage multiple schools.

## API Documentation

Swagger documentation is available at `/swagger` in development mode.

## Version History

| Version | Content | Date |
|---------|---------|------|
| V3.0 | Upgrade to .NET 10 | 2026 |
| V2.0 | Front website | 2022 |
| V1.3 | Librarian Module | 2021 |
| V1.2 | Reception Module | 2020 |
| V1.1 | Zoom Integration | 2020 |
| V1.0 | Initial version | 2020 |

## License

This project is proprietary software.
