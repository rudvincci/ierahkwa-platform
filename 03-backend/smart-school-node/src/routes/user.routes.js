const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const config = require('../config/config');
const { User, Role, UserRole } = require('../models');
const bcrypt = require('bcryptjs');

const { ADMIN, SCHOOL_ADMIN } = config.roles;

// Get all users
router.get('/', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const where = {};
    if (req.tenantId && !req.roles.includes(ADMIN)) {
      where.tenantId = req.tenantId;
    }
    
    const users = await User.findAll({
      where,
      include: [{ model: Role, through: { attributes: [] } }],
      order: [['createdAt', 'DESC']]
    });
    
    res.json({ success: true, data: users });
  } catch (error) {
    next(error);
  }
});

// Get user by ID
router.get('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const user = await User.findByPk(req.params.id, {
      include: [{ model: Role, through: { attributes: [] } }]
    });
    
    if (!user) {
      return res.status(404).json({ success: false, message: 'User not found' });
    }
    
    res.json({ success: true, data: user });
  } catch (error) {
    next(error);
  }
});

// Create user
router.post('/', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const { username, email, password, firstName, lastName, phone, roles, tenantId } = req.body;
    
    const user = await User.create({
      username,
      email,
      password,
      firstName,
      lastName,
      phone,
      tenantId: tenantId || req.tenantId
    });
    
    // Assign roles
    if (roles && roles.length > 0) {
      const roleRecords = await Role.findAll({ where: { name: roles } });
      for (const role of roleRecords) {
        await UserRole.create({ userId: user.id, roleId: role.id });
      }
    }
    
    const createdUser = await User.findByPk(user.id, {
      include: [{ model: Role, through: { attributes: [] } }]
    });
    
    res.status(201).json({ success: true, data: createdUser });
  } catch (error) {
    next(error);
  }
});

// Update user
router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const user = await User.findByPk(req.params.id);
    
    if (!user) {
      return res.status(404).json({ success: false, message: 'User not found' });
    }
    
    const { firstName, lastName, phone, address, isActive, roles } = req.body;
    
    await user.update({ firstName, lastName, phone, address, isActive });
    
    // Update roles if provided
    if (roles) {
      await UserRole.destroy({ where: { userId: user.id } });
      const roleRecords = await Role.findAll({ where: { name: roles } });
      for (const role of roleRecords) {
        await UserRole.create({ userId: user.id, roleId: role.id });
      }
    }
    
    const updatedUser = await User.findByPk(user.id, {
      include: [{ model: Role, through: { attributes: [] } }]
    });
    
    res.json({ success: true, data: updatedUser });
  } catch (error) {
    next(error);
  }
});

// Delete user
router.delete('/:id', authorize(ADMIN), async (req, res, next) => {
  try {
    const user = await User.findByPk(req.params.id);
    
    if (!user) {
      return res.status(404).json({ success: false, message: 'User not found' });
    }
    
    await user.destroy();
    res.json({ success: true, message: 'User deleted successfully' });
  } catch (error) {
    next(error);
  }
});

// Change password
router.post('/:id/change-password', async (req, res, next) => {
  try {
    const { currentPassword, newPassword } = req.body;
    
    // Users can only change their own password unless admin
    if (req.userId !== parseInt(req.params.id) && !req.roles.includes(ADMIN)) {
      return res.status(403).json({ success: false, message: 'Access denied' });
    }
    
    const user = await User.findByPk(req.params.id);
    
    if (!user) {
      return res.status(404).json({ success: false, message: 'User not found' });
    }
    
    // Validate current password (skip for admin)
    if (!req.roles.includes(ADMIN)) {
      const isValid = await user.validatePassword(currentPassword);
      if (!isValid) {
        return res.status(400).json({ success: false, message: 'Current password is incorrect' });
      }
    }
    
    user.password = newPassword;
    await user.save();
    
    res.json({ success: true, message: 'Password changed successfully' });
  } catch (error) {
    next(error);
  }
});

module.exports = router;
