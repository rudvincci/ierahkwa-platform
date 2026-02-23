# Smart School & Accounting System - Node.js

A complete school management system with integrated accounting built with Node.js, Express, and Sequelize.

## Features

### Academic Management
- **Student Management** - Full lifecycle from admission to graduation
- **Teacher Management** - Staff profiles and subject assignments
- **Grade & Classroom** - Organize students into grades and classes
- **Schedule Management** - Weekly class schedules
- **Homework System** - Create assignments, upload content, grade submissions
- **Live Classes** - Zoom integration for online classes

### Financial Management
- **Fee Management** - Configure tuition and other fees
- **Invoice System** - Generate fee invoices and track payments
- **Chart of Accounts** - Full double-entry accounting
- **Journal Entries** - Record financial transactions
- **Product/Inventory** - Track school supplies and products

### Reception & Administration
- **Admission Enquiries** - Track prospective students
- **Visitor Book** - Log school visitors
- **Phone Call Log** - Record incoming/outgoing calls
- **Postal Management** - Track incoming/outgoing mail
- **Complaint Management** - Handle student/parent complaints

### Library Management
- **Book Catalog** - Manage library books
- **Member Management** - Library card system
- **Borrow/Return** - Track book borrowing
- **Fine Calculation** - Automatic overdue fines

### System Features
- **Multi-Tenant** - Support multiple schools
- **Role-Based Access** - Admin, Teacher, Student, Parent, etc.
- **JWT Authentication** - Secure API access
- **Audit Logging** - Track system changes

## Tech Stack

- **Runtime**: Node.js 18+
- **Framework**: Express.js
- **ORM**: Sequelize
- **Database**: PostgreSQL / MySQL
- **Auth**: JWT (jsonwebtoken)
- **Views**: EJS
- **Validation**: express-validator

## Installation

1. **Clone and install dependencies**
```bash
cd smart-school-node
npm install
```

2. **Configure environment**
```bash
cp .env.example .env
# Edit .env with your database credentials
```

3. **Create database**
```bash
# PostgreSQL
createdb smart_school_db

# Or MySQL
mysql -u root -p -e "CREATE DATABASE smart_school_db"
```

4. **Run database seed**
```bash
node src/seeders/seed.js
```

5. **Start the server**
```bash
# Development
npm run dev

# Production
npm start
```

6. **Access the application**
- Web UI: http://localhost:3000
- API: http://localhost:3000/api
- API Docs: http://localhost:3000/api-docs

## Default Credentials

| Role | Username | Password |
|------|----------|----------|
| System Admin | admin | P@ssw0rd |
| School Admin | schooladmin | P@ssw0rd |

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/login | User login |
| POST | /api/auth/refresh | Refresh token |
| POST | /api/auth/logout | User logout |

### Students
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/students | List all students |
| GET | /api/students/:id | Get student by ID |
| POST | /api/students | Create student |
| PUT | /api/students/:id | Update student |
| DELETE | /api/students/:id | Delete student |

### Teachers
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/teachers | List all teachers |
| POST | /api/teachers | Create teacher |

### Invoices
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/invoices | List invoices |
| POST | /api/invoices | Create invoice |
| POST | /api/invoices/:id/payment | Add payment |

### Homework
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/homeworks | List homework |
| POST | /api/homeworks | Create homework |
| POST | /api/homeworks/:id/submit | Submit answers (Student) |
| POST | /api/homeworks/:id/grade/:answerId | Grade submission |

### Live Classes
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/live-classes | List classes |
| POST | /api/live-classes | Create class |
| POST | /api/live-classes/:id/join | Join class (Student) |

### Library
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/library/books | List books |
| POST | /api/library/borrow | Borrow book |
| POST | /api/library/return/:id | Return book |

## Project Structure

```
smart-school-node/
├── src/
│   ├── config/          # Configuration files
│   ├── controllers/     # Route controllers
│   ├── middleware/      # Express middleware
│   ├── models/          # Sequelize models
│   ├── routes/          # API routes
│   ├── seeders/         # Database seeders
│   ├── utils/           # Utility functions
│   ├── views/           # EJS templates
│   └── server.js        # Entry point
├── uploads/             # File uploads
├── logs/                # Application logs
├── package.json
└── README.md
```

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| PORT | Server port | 3000 |
| NODE_ENV | Environment | development |
| DB_HOST | Database host | localhost |
| DB_PORT | Database port | 5432 |
| DB_NAME | Database name | smart_school_db |
| DB_USER | Database user | postgres |
| DB_PASSWORD | Database password | - |
| DB_DIALECT | Database type | postgres |
| JWT_SECRET | JWT signing secret | - |
| JWT_EXPIRES_IN | Token expiry | 1h |

## User Roles

- **Admin** - Full system access
- **SchoolAdmin** - School management
- **Accountant** - Financial operations
- **Teacher** - Class and homework management
- **Student** - View schedule, submit homework
- **Parent** - View student progress
- **Receptionist** - Front desk operations
- **Librarian** - Library management

## License

ISC License

## Support

For issues and feature requests, please create an issue in the repository.
