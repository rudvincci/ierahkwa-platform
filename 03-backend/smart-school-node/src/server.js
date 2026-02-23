require('dotenv').config();
const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const morgan = require('morgan');
const path = require('path');
const session = require('express-session');
const cookieParser = require('cookie-parser');
const flash = require('connect-flash');

const { sequelize } = require('./models');
const logger = require('./utils/logger');

// Import routes
const authRoutes = require('./routes/auth.routes');
const userRoutes = require('./routes/user.routes');
const tenantRoutes = require('./routes/tenant.routes');
const gradeRoutes = require('./routes/grade.routes');
const classRoomRoutes = require('./routes/classroom.routes');
const materialRoutes = require('./routes/material.routes');
const teacherRoutes = require('./routes/teacher.routes');
const studentRoutes = require('./routes/student.routes');
const parentRoutes = require('./routes/parent.routes');
const scheduleRoutes = require('./routes/schedule.routes');
const homeworkRoutes = require('./routes/homework.routes');
const feesRoutes = require('./routes/fees.routes');
const invoiceRoutes = require('./routes/invoice.routes');
const productRoutes = require('./routes/product.routes');
const journalRoutes = require('./routes/journal.routes');
const accountRoutes = require('./routes/account.routes');
const receptionRoutes = require('./routes/reception.routes');
const libraryRoutes = require('./routes/library.routes');
const liveClassRoutes = require('./routes/liveclass.routes');
const dashboardRoutes = require('./routes/dashboard.routes');

// Import middleware
const { authMiddleware } = require('./middleware/auth.middleware');
const { tenantMiddleware } = require('./middleware/tenant.middleware');
const errorHandler = require('./middleware/error.middleware');

const app = express();

// Security middleware
const { corsConfig } = require('../../shared/security');
app.use(helmet());
app.use(cors(corsConfig()));

// Body parser
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(cookieParser());

// Logging
app.use(morgan('combined', { stream: { write: message => logger.info(message.trim()) } }));

// Session
app.use(session({
  secret: process.env.SESSION_SECRET || 'secret',
  resave: false,
  saveUninitialized: false,
  cookie: { secure: process.env.NODE_ENV === 'production', maxAge: 3600000 }
}));
app.use(flash());

// View engine
app.set('view engine', 'ejs');
app.set('views', path.join(__dirname, 'views'));

// Static files
app.use(express.static(path.join(__dirname, 'public')));
app.use('/uploads', express.static(path.join(__dirname, '../uploads')));

// Global variables for views
app.use((req, res, next) => {
  res.locals.success_msg = req.flash('success_msg');
  res.locals.error_msg = req.flash('error_msg');
  res.locals.error = req.flash('error');
  res.locals.user = req.user || null;
  next();
});

// API Routes
app.use('/api/auth', authRoutes);
app.use('/api/users', authMiddleware, userRoutes);
app.use('/api/tenants', authMiddleware, tenantRoutes);
app.use('/api/grades', authMiddleware, tenantMiddleware, gradeRoutes);
app.use('/api/classrooms', authMiddleware, tenantMiddleware, classRoomRoutes);
app.use('/api/materials', authMiddleware, tenantMiddleware, materialRoutes);
app.use('/api/teachers', authMiddleware, tenantMiddleware, teacherRoutes);
app.use('/api/students', authMiddleware, tenantMiddleware, studentRoutes);
app.use('/api/parents', authMiddleware, tenantMiddleware, parentRoutes);
app.use('/api/schedules', authMiddleware, tenantMiddleware, scheduleRoutes);
app.use('/api/homeworks', authMiddleware, tenantMiddleware, homeworkRoutes);
app.use('/api/fees', authMiddleware, tenantMiddleware, feesRoutes);
app.use('/api/invoices', authMiddleware, tenantMiddleware, invoiceRoutes);
app.use('/api/products', authMiddleware, tenantMiddleware, productRoutes);
app.use('/api/journals', authMiddleware, tenantMiddleware, journalRoutes);
app.use('/api/accounts', authMiddleware, tenantMiddleware, accountRoutes);
app.use('/api/reception', authMiddleware, tenantMiddleware, receptionRoutes);
app.use('/api/library', authMiddleware, tenantMiddleware, libraryRoutes);
app.use('/api/live-classes', authMiddleware, tenantMiddleware, liveClassRoutes);
app.use('/api/dashboard', authMiddleware, dashboardRoutes);

// Web Routes
app.use('/', require('./routes/web.routes'));

// Error handling
app.use(errorHandler);

// 404 handler
app.use((req, res) => {
  res.status(404).json({ success: false, message: 'Route not found' });
});

const PORT = process.env.PORT || 3000;

// Database connection and server start
sequelize.authenticate()
  .then(() => {
    logger.info('Database connected successfully');
    return sequelize.sync({ alter: process.env.NODE_ENV === 'development' });
  })
  .then(() => {
    logger.info('Database synchronized');
    app.listen(PORT, () => {
      logger.info(`Server running on port ${PORT}`);
      console.log(`
╔═══════════════════════════════════════════════════════════╗
║     Smart School & Accounting System v3.0                 ║
║     Running on http://localhost:${PORT}                      ║
║     Environment: ${process.env.NODE_ENV || 'development'}                          ║
╚═══════════════════════════════════════════════════════════╝
      `);
    });
  })
  .catch(err => {
    logger.error('Unable to connect to database:', err);
    process.exit(1);
  });

module.exports = app;
