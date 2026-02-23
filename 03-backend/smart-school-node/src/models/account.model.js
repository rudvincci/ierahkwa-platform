const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Account = sequelize.define('Account', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    code: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    name: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    nameAr: DataTypes.STRING(200),
    type: {
      type: DataTypes.ENUM('Asset', 'Liability', 'Equity', 'Revenue', 'Expense'),
      allowNull: false
    },
    parentId: DataTypes.INTEGER,
    level: {
      type: DataTypes.INTEGER,
      defaultValue: 1
    },
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    isSystemAccount: {
      type: DataTypes.BOOLEAN,
      defaultValue: false
    },
    openingBalance: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    currentBalance: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'accounts',
    timestamps: true,
    paranoid: true
  });

  return Account;
};
