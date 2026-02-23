const jwt = require('jsonwebtoken');
const bcrypt = require('bcryptjs');
const { v4: uuidv4 } = require('uuid');
const { validationResult } = require('express-validator');
const { User, Role, Tenant, AuditLog } = require('../models');
const config = require('../config/config');
const logger = require('../utils/logger');

// Generate tokens
const generateTokens = (user, roles) => {
  const payload = {
    id: user.id,
    username: user.username,
    email: user.email,
    tenantId: user.tenantId,
    roles
  };
  
  const token = jwt.sign(payload, config.jwt.secret, { expiresIn: config.jwt.expiresIn });
  const refreshToken = uuidv4() + uuidv4();
  
  return { token, refreshToken };
};

exports.login = async (req, res, next) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ success: false, errors: errors.array() });
    }
    
    const { username, password } = req.body;
    
    // Find user with roles
    const user = await User.findOne({
      where: { username },
      include: [{ model: Role, through: { attributes: [] } }]
    });
    
    if (!user) {
      return res.status(401).json({ success: false, message: 'Invalid credentials' });
    }
    
    // Validate password
    const isValid = await user.validatePassword(password);
    if (!isValid) {
      return res.status(401).json({ success: false, message: 'Invalid credentials' });
    }
    
    if (!user.isActive) {
      return res.status(401).json({ success: false, message: 'Account is deactivated' });
    }
    
    // Generate tokens
    const roles = user.Roles.map(r => r.name);
    const { token, refreshToken } = generateTokens(user, roles);
    
    // Save refresh token
    const refreshExpiry = new Date();
    refreshExpiry.setDate(refreshExpiry.getDate() + 7);
    
    await user.update({
      refreshToken,
      refreshTokenExpiry: refreshExpiry,
      lastLoginAt: new Date()
    });
    
    // Get tenant info
    let tenant = null;
    if (user.tenantId) {
      tenant = await Tenant.findByPk(user.tenantId);
    }
    
    // Log audit
    await AuditLog.create({
      userId: user.id.toString(),
      userName: user.username,
      action: 'Login',
      entityName: 'User',
      entityId: user.id.toString(),
      ipAddress: req.ip,
      userAgent: req.headers['user-agent'],
      tenantId: user.tenantId
    });
    
    res.json({
      success: true,
      token,
      refreshToken,
      user: {
        id: user.id,
        username: user.username,
        email: user.email,
        firstName: user.firstName,
        lastName: user.lastName,
        roles,
        tenantId: user.tenantId,
        tenantName: tenant?.name
      }
    });
  } catch (error) {
    next(error);
  }
};

exports.refreshToken = async (req, res, next) => {
  try {
    const { refreshToken } = req.body;
    
    if (!refreshToken) {
      return res.status(400).json({ success: false, message: 'Refresh token required' });
    }
    
    const user = await User.findOne({
      where: { refreshToken },
      include: [{ model: Role, through: { attributes: [] } }]
    });
    
    if (!user || user.refreshTokenExpiry < new Date()) {
      return res.status(401).json({ success: false, message: 'Invalid or expired refresh token' });
    }
    
    // Generate new tokens
    const roles = user.Roles.map(r => r.name);
    const tokens = generateTokens(user, roles);
    
    // Update refresh token
    const refreshExpiry = new Date();
    refreshExpiry.setDate(refreshExpiry.getDate() + 7);
    
    await user.update({
      refreshToken: tokens.refreshToken,
      refreshTokenExpiry: refreshExpiry
    });
    
    res.json({
      success: true,
      token: tokens.token,
      refreshToken: tokens.refreshToken
    });
  } catch (error) {
    next(error);
  }
};

exports.logout = async (req, res, next) => {
  try {
    const token = req.headers.authorization?.split(' ')[1];
    
    if (token) {
      try {
        const decoded = jwt.verify(token, config.jwt.secret);
        await User.update(
          { refreshToken: null, refreshTokenExpiry: null },
          { where: { id: decoded.id } }
        );
        
        await AuditLog.create({
          userId: decoded.id.toString(),
          userName: decoded.username,
          action: 'Logout',
          entityName: 'User',
          entityId: decoded.id.toString(),
          ipAddress: req.ip,
          userAgent: req.headers['user-agent'],
          tenantId: decoded.tenantId
        });
      } catch {}
    }
    
    res.clearCookie('authToken');
    res.json({ success: true, message: 'Logged out successfully' });
  } catch (error) {
    next(error);
  }
};

exports.forgotPassword = async (req, res, next) => {
  try {
    const { email } = req.body;
    
    // Always return success to prevent email enumeration
    res.json({ 
      success: true, 
      message: 'If the email exists, a reset link will be sent' 
    });
    
    // TODO: Implement email sending
    const user = await User.findOne({ where: { email } });
    if (user) {
      // Generate reset token and send email
      logger.info(`Password reset requested for: ${email}`);
    }
  } catch (error) {
    next(error);
  }
};

exports.resetPassword = async (req, res, next) => {
  try {
    const { token, newPassword } = req.body;
    
    // TODO: Implement token validation and password reset
    res.json({ success: true, message: 'Password reset successfully' });
  } catch (error) {
    next(error);
  }
};
