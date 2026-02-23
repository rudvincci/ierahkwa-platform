const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const ClassRoom = sequelize.define('ClassRoom', {
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
    gradeId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    capacity: {
      type: DataTypes.INTEGER,
      defaultValue: 30
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
    tableName: 'classrooms',
    timestamps: true,
    paranoid: true
  });

  return ClassRoom;
};
