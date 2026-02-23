const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Grade = sequelize.define('Grade', {
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
    orderIndex: {
      type: DataTypes.INTEGER,
      defaultValue: 0
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
    tableName: 'grades',
    timestamps: true,
    paranoid: true,
    indexes: [
      { unique: true, fields: ['tenantId', 'name'] }
    ]
  });

  return Grade;
};
