const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Unit = sequelize.define('Unit', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    name: {
      type: DataTypes.STRING(100),
      allowNull: false
    },
    shortName: DataTypes.STRING(20),
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'units',
    timestamps: true,
    paranoid: true
  });

  return Unit;
};
