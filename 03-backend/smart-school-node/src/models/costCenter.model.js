const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const CostCenter = sequelize.define('CostCenter', {
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
    description: DataTypes.TEXT,
    parentId: DataTypes.INTEGER,
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'cost_centers',
    timestamps: true,
    paranoid: true
  });

  return CostCenter;
};
