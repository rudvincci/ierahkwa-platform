const { Tenant } = require('../models');
const config = require('../config/config');

const tenantMiddleware = async (req, res, next) => {
  try {
    // Super admin (Admin role) can access without tenant
    if (req.roles && req.roles.includes(config.roles.ADMIN)) {
      // If tenant is specified in header or query, use it
      const tenantId = req.headers['x-tenant-id'] || req.query.tenantId;
      if (tenantId) {
        const tenant = await Tenant.findByPk(tenantId);
        if (tenant && tenant.isActive) {
          req.tenantId = parseInt(tenantId);
        }
      }
      return next();
    }
    
    // Other users must have a tenant
    if (!req.tenantId) {
      return res.status(403).json({ 
        success: false, 
        message: 'Tenant not assigned to user' 
      });
    }
    
    // Verify tenant exists and is active
    const tenant = await Tenant.findByPk(req.tenantId);
    if (!tenant || !tenant.isActive) {
      return res.status(403).json({ 
        success: false, 
        message: 'Tenant not found or inactive' 
      });
    }
    
    req.tenant = tenant;
    next();
  } catch (error) {
    return res.status(500).json({ 
      success: false, 
      message: 'Error processing tenant' 
    });
  }
};

const addTenantFilter = (req) => {
  if (req.tenantId) {
    return { tenantId: req.tenantId };
  }
  return {};
};

module.exports = { tenantMiddleware, addTenantFilter };
