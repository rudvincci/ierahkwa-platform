const jwt = require('jsonwebtoken');
const { User, Role } = require('../models');
const config = require('../config/config');

const authMiddleware = async (req, res, next) => {
  try {
    // Get token from header or cookie
    let token = req.headers.authorization?.split(' ')[1] || req.cookies?.authToken;
    
    if (!token) {
      return res.status(401).json({ success: false, message: 'No token provided' });
    }
    
    // Verify token
    const decoded = jwt.verify(token, config.jwt.secret);
    
    // Get user with roles
    const user = await User.findByPk(decoded.id, {
      include: [{ model: Role, through: { attributes: [] } }]
    });
    
    if (!user || !user.isActive) {
      return res.status(401).json({ success: false, message: 'Invalid or inactive user' });
    }
    
    // Attach user and roles to request
    req.user = user;
    req.userId = user.id;
    req.tenantId = user.tenantId;
    req.roles = user.Roles.map(r => r.name);
    
    next();
  } catch (error) {
    if (error.name === 'TokenExpiredError') {
      return res.status(401).json({ success: false, message: 'Token expired' });
    }
    return res.status(401).json({ success: false, message: 'Invalid token' });
  }
};

const authorize = (...allowedRoles) => {
  return (req, res, next) => {
    if (!req.roles || req.roles.length === 0) {
      return res.status(403).json({ success: false, message: 'No roles assigned' });
    }
    
    const hasRole = req.roles.some(role => allowedRoles.includes(role));
    
    if (!hasRole) {
      return res.status(403).json({ success: false, message: 'Access denied' });
    }
    
    next();
  };
};

const optionalAuth = async (req, res, next) => {
  try {
    let token = req.headers.authorization?.split(' ')[1] || req.cookies?.authToken;
    
    if (token) {
      const decoded = jwt.verify(token, config.jwt.secret);
      const user = await User.findByPk(decoded.id, {
        include: [{ model: Role, through: { attributes: [] } }]
      });
      
      if (user && user.isActive) {
        req.user = user;
        req.userId = user.id;
        req.tenantId = user.tenantId;
        req.roles = user.Roles.map(r => r.name);
      }
    }
    next();
  } catch {
    next();
  }
};

module.exports = { authMiddleware, authorize, optionalAuth };
