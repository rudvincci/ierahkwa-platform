const logger = require('../utils/logger');
const { AuditLog } = require('../models');

const errorHandler = async (err, req, res, next) => {
  // Log error
  logger.error({
    message: err.message,
    stack: err.stack,
    path: req.path,
    method: req.method,
    userId: req.userId,
    tenantId: req.tenantId
  });
  
  // Log to database
  try {
    await AuditLog.create({
      userId: req.userId?.toString(),
      userName: req.user?.username,
      action: 'Error',
      entityName: 'System',
      oldValues: { path: req.path, method: req.method },
      newValues: { error: err.message },
      ipAddress: req.ip,
      userAgent: req.headers['user-agent'],
      tenantId: req.tenantId
    });
  } catch (logError) {
    logger.error('Failed to log error to database:', logError);
  }
  
  // Handle specific errors
  if (err.name === 'SequelizeValidationError') {
    return res.status(400).json({
      success: false,
      message: 'Validation error',
      errors: err.errors.map(e => e.message)
    });
  }
  
  if (err.name === 'SequelizeUniqueConstraintError') {
    return res.status(409).json({
      success: false,
      message: 'Duplicate entry',
      errors: err.errors.map(e => e.message)
    });
  }
  
  if (err.name === 'SequelizeForeignKeyConstraintError') {
    return res.status(400).json({
      success: false,
      message: 'Invalid reference'
    });
  }
  
  // Default error response
  const statusCode = err.statusCode || 500;
  const message = process.env.NODE_ENV === 'production' && statusCode === 500
    ? 'Internal server error'
    : err.message;
  
  res.status(statusCode).json({
    success: false,
    message,
    ...(process.env.NODE_ENV !== 'production' && { stack: err.stack })
  });
};

module.exports = errorHandler;
