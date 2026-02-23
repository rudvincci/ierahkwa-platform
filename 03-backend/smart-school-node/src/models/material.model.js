const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Material = sequelize.define('Material', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    name: {
      type: DataTypes.STRING(100),
      allowNull: false
    },
    description: DataTypes.TEXT,
    code: DataTypes.STRING(50),
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'materials',
    timestamps: true,
    paranoid: true
  });

  return Material;
};
