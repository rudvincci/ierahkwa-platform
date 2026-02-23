const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const AuditLog = sequelize.define('AuditLog', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    userId: DataTypes.STRING(50),
    userName: DataTypes.STRING(100),
    action: {
      type: DataTypes.STRING(100),
      allowNull: false
    },
    entityName: DataTypes.STRING(100),
    entityId: DataTypes.STRING(50),
    oldValues: DataTypes.JSON,
    newValues: DataTypes.JSON,
    ipAddress: DataTypes.STRING(50),
    userAgent: DataTypes.TEXT,
    tenantId: DataTypes.INTEGER
  }, {
    tableName: 'audit_logs',
    timestamps: true,
    updatedAt: false
  });

  return AuditLog;
};
