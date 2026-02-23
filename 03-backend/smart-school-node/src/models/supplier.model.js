const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Supplier = sequelize.define('Supplier', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    name: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    code: DataTypes.STRING(50),
    email: DataTypes.STRING(200),
    phone: DataTypes.STRING(50),
    address: DataTypes.TEXT,
    taxNumber: DataTypes.STRING(50),
    balance: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    isDefault: {
      type: DataTypes.BOOLEAN,
      defaultValue: false
    },
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'suppliers',
    timestamps: true,
    paranoid: true
  });

  return Supplier;
};
