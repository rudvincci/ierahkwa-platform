const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Tenant = sequelize.define('Tenant', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    name: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    code: {
      type: DataTypes.STRING(50),
      unique: true
    },
    address: DataTypes.TEXT,
    phone: DataTypes.STRING(50),
    email: DataTypes.STRING(200),
    logo: DataTypes.STRING(500),
    website: DataTypes.STRING(200),
    currency: {
      type: DataTypes.STRING(10),
      defaultValue: 'USD'
    },
    taxRate: {
      type: DataTypes.DECIMAL(5, 2),
      defaultValue: 0
    },
    openingCash: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    defaultLanguage: {
      type: DataTypes.STRING(10),
      defaultValue: 'en'
    },
    timeZone: {
      type: DataTypes.STRING(50),
      defaultValue: 'UTC'
    },
    dateFormat: {
      type: DataTypes.STRING(20),
      defaultValue: 'YYYY-MM-DD'
    }
  }, {
    tableName: 'tenants',
    timestamps: true,
    paranoid: true
  });

  return Tenant;
};
